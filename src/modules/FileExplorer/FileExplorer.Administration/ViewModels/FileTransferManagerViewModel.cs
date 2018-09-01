using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Threading;
using FileExplorer.Administration.Converters;
using FileExplorer.Administration.Models;
using FileExplorer.Administration.Rest;
using FileExplorer.Administration.ViewModels.Explorer;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.StatusBar;
using Orcus.Utilities;
using Prism.Commands;
using Prism.Mvvm;
using Unclassified.TxLib;

namespace FileExplorer.Administration.ViewModels
{
    public class FileTransferManagerViewModel : BindableBase, IFileExplorerChildViewModel
    {
        private IPackageRestClient _restClient;
        private FileExplorerViewModel _baseVm;
        private readonly ObservableCollection<FileTransferViewModel> _transfers;
        private bool _isBusy;
        private double _progress;
        private double _totalSpeed;
        private readonly SemaphoreSlim _concurrentUploadsSemaphore;
        private readonly SemaphoreSlim _concurrentDownloadsSemaphore;
        private readonly SemaphoreSlim _statusBarLock;
        private ProgressStatusMessage _progressStatusMessage;
        private readonly List<FileTransferViewModel> _activeTransfers;
        private readonly object _activeTransfersLock = new object();

        public FileTransferManagerViewModel()
        {
            _transfers = new ObservableCollection<FileTransferViewModel>();
            _concurrentUploadsSemaphore = new SemaphoreSlim(3, 3);
            _concurrentDownloadsSemaphore = new SemaphoreSlim(3, 3);
            _statusBarLock = new SemaphoreSlim(1, 1);
            _activeTransfers = new List<FileTransferViewModel>();

            TransfersView = new ListCollectionView(_transfers);
            TransfersView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(FileTransferViewModel.State),
                new IsTransferFinishedConverter()));
            TransfersView.LiveGroupingProperties.Add(nameof(FileTransferViewModel.State));
            TransfersView.IsLiveGrouping = true;
            TransfersView.SortDescriptions.Add(new SortDescription(nameof(FileTransferViewModel.Timestamp),
                ListSortDirection.Descending));
        }

        public void Initialize(FileExplorerViewModel fileExplorerViewModel)
        {
            _restClient = fileExplorerViewModel.RestClient;
            _baseVm = fileExplorerViewModel;
        }

        public ListCollectionView TransfersView { get; }

        public double Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public double TotalSpeed
        {
            get => _totalSpeed;
            set => SetProperty(ref _totalSpeed, value);
        }

        private bool _isFlyoutOpen;

        public bool IsFlyoutOpen
        {
            get => _isFlyoutOpen;
            set => SetProperty(ref _isFlyoutOpen, value);
        }

        private DelegateCommand _openDownloadsFlyoutCommand;

        public DelegateCommand OpenDownloadsFlyoutCommand
        {
            get
            {
                return _openDownloadsFlyoutCommand ?? (_openDownloadsFlyoutCommand = new DelegateCommand(() =>
                           {
                               IsFlyoutOpen = true;
                           }));
            }
        }

        public Task ExecuteTransfer(FileTransferViewModel transfer)
        {
            _transfers.Add(transfer);

            if (transfer.IsUpload)
                return Task.Run(() => InternalExecuteUpload(transfer));

            return Task.Run(() => InternalExecuteDownload(transfer));
        }

        private async Task InternalExecuteDownload(FileTransferViewModel transfer)
        {
            if (transfer.IsUpload)
                throw new InvalidOperationException("The transfer must be a download");

            await _concurrentDownloadsSemaphore.WaitAsync();
            try
            {

            }
            finally
            {
                _concurrentDownloadsSemaphore.Release();
            }
        }

        private async Task InternalExecuteUpload(FileTransferViewModel transfer)
        {
            if (!transfer.IsUpload)
                throw new InvalidOperationException("The transfer must be an upload");

            lock (_activeTransfersLock)
                _activeTransfers.Add(transfer);

            try
            {
                await _concurrentUploadsSemaphore.WaitAsync();
                try
                {
                    transfer.State = FileTransferState.Preparing;

                    var fs = transfer.IsDirectory
                        ? (FileSystemInfo) new DirectoryInfo(transfer.SourcePath)
                        : new FileInfo(transfer.SourcePath);

                    if (!fs.Exists)
                    {
                        transfer.State = FileTransferState.NotFound;
                        return;
                    }

                    var processingEntry = new ProcessingEntryViewModel(transfer, _baseVm.FileSystem, _baseVm);
                    _baseVm.Dispatcher.Current.BeginInvoke(
                        new Action(() => _baseVm.ProcessingEntries.Add(processingEntry)),
                        DispatcherPriority.ApplicationIdle).Task.Forget();

                    var zipContent = new ZipContent(fs);
                    zipContent.ProgressChanged += (sender, args) =>
                    {
                        transfer.State = FileTransferState.Transferring;

                        transfer.Progress = args.Progress;
                        transfer.TotalSize = args.TotalSize;
                        transfer.ProcessedSize = args.ProcessedSize;
                        transfer.CurrentSpeed = args.Speed;
                        transfer.EstimatedRemainingTime = args.EstimatedTime;

                        processingEntry.Progress = args.Progress;
                        processingEntry.SetSize(args.ProcessedSize);

                        UpdateStatus();
                    };

                    var isRemoved = false;
                    try
                    {
                        await FileExplorerResource.Upload(zipContent, transfer.TargetPath, transfer.CancellationToken,
                            _restClient);

                        await _baseVm.Dispatcher.Current.BeginInvoke(new Action(() =>
                        {
                            transfer.State = FileTransferState.Succeeded;
                            isRemoved = true;
                            _baseVm.ProcessingEntries.Remove(processingEntry);
                            _baseVm.FileSystem.UploadCompleted(processingEntry.Source);
                        }));
                    }
                    catch (OperationCanceledException)
                    {
                        transfer.State = FileTransferState.Canceled;
                    }
                    catch (Exception)
                    {
                        transfer.State = FileTransferState.Failed;
                    }
                    finally
                    {
                        if (!isRemoved)
                            _baseVm.Dispatcher.Current
                                .BeginInvoke(new Action(() => _baseVm.ProcessingEntries.Remove(processingEntry)),
                                    DispatcherPriority.ApplicationIdle).Task.Forget();
                    }
                }
                finally
                {
                    _concurrentUploadsSemaphore.Release();
                }
            }
            finally
            {
                lock (_activeTransfersLock)
                    _activeTransfers.Remove(transfer);
            }

            UpdateStatus();
        }

        private void UpdateStatus()
        {
            if (_statusBarLock.Wait(0))
            {
                try
                {
                    lock (_activeTransfersLock)
                    {
                        if (_activeTransfers.Count == 0)
                        {
                            if (IsBusy)
                            {
                                IsBusy = false;
                                _progressStatusMessage?.Dispose();
                                _progressStatusMessage = null;
                                Progress = 0;

                                _baseVm.StatusBar.ShowSuccess(Tx.T("FileExplorer:StatusBar.TransmissionCompleted"));
                            }
                        }
                        else
                        {
                            IsBusy = true;
                            if (_progressStatusMessage == null)
                            {
                                _progressStatusMessage =
                                    new ProgressStatusMessage(null) {Animation = StatusBarAnimation.Send};
                                _baseVm.StatusBar.PushStatus(_progressStatusMessage);
                            }

                            Progress = _activeTransfers.Sum(x => x.Progress) / _activeTransfers.Count;
                            _progressStatusMessage.Progress = Progress;

                            TotalSpeed = _activeTransfers.Sum(x => x.CurrentSpeed);
                            _progressStatusMessage.Message = GetTransmissionStatusMessage() + " " + Tx.DataSize((long) TotalSpeed) + "/s";
                        }
                    }
                }
                finally
                {
                    _statusBarLock.Release();
                }
            }
        }

        private string GetTransmissionStatusMessage()
        {
            var directoriesCount = _activeTransfers.Count(x => x.IsDirectory);
            var filesCount = _activeTransfers.Count - directoriesCount;

            if (directoriesCount == 0)
            {
                return Tx.T("FileExplorer:StatusBar.Transmission.Files", _activeTransfers.Count, "name",
                    _activeTransfers[0].Name);
            }

            if (filesCount == 0)
                return Tx.T("FileExplorer:StatusBar.Transmission.Directories", _activeTransfers.Count, "name",
                    _activeTransfers[0].Name);

            return Tx.T("FileExplorer:StatusBar.Transmission.FilesAndDirectories", "files",
                Tx.T("FileExplorer:StatusBar.Transmission.FilesAndDirectories.Files", filesCount), "directories",
                Tx.T("FileExplorer:StatusBar.Transmission.FilesAndDirectories.Directories", directoriesCount));
        }
    }
}