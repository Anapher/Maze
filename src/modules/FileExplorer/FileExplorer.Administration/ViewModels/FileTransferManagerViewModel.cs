using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
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
using Orcus.Administration.Library.Logging;
using Orcus.Administration.Library.StatusBar;
using Orcus.Utilities;
using Prism.Commands;
using Prism.Mvvm;
using Unclassified.TxLib;

namespace FileExplorer.Administration.ViewModels
{
    public class FileTransferManagerViewModel : BindableBase, IFileExplorerChildViewModel
    {
        private static readonly ILog Logger = LogProvider.For<FileTransferViewModel>();

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
        private bool _isFlyoutOpen;
        private DelegateCommand _openDownloadsFlyoutCommand;

        public FileTransferManagerViewModel()
        {
            _transfers = new ObservableCollection<FileTransferViewModel>();
            _concurrentUploadsSemaphore = new SemaphoreSlim(3, 3);
            _concurrentDownloadsSemaphore = new SemaphoreSlim(3, 3);
            _statusBarLock = new SemaphoreSlim(1, 1);
            _activeTransfers = new List<FileTransferViewModel>();

            TransfersView = new ListCollectionView(_transfers);
            TransfersView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(FileTransferViewModel.IsCompleted)));
            TransfersView.LiveGroupingProperties.Add(nameof(FileTransferViewModel.IsCompleted));
            TransfersView.IsLiveGrouping = true;
            TransfersView.SortDescriptions.Add(new SortDescription(nameof(FileTransferViewModel.IsCompleted),
                ListSortDirection.Ascending));
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

        public bool IsFlyoutOpen
        {
            get => _isFlyoutOpen;
            set => SetProperty(ref _isFlyoutOpen, value);
        }

        public DelegateCommand OpenDownloadsFlyoutCommand
        {
            get
            {
                return _openDownloadsFlyoutCommand ?? (_openDownloadsFlyoutCommand =
                           new DelegateCommand(() => { IsFlyoutOpen = !IsFlyoutOpen; }));
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

            lock (_activeTransfersLock)
                _activeTransfers.Add(transfer);
            try
            {
                await _concurrentDownloadsSemaphore.WaitAsync();
                try
                {
                    transfer.State = FileTransferState.Preparing;
                    UpdateStatus();

                    var streamCopyUtil = new StreamCopyUtil();
                    streamCopyUtil.ProgressChanged += (sender, args) =>
                    {
                        transfer.UpdateProgress(args);
                        UpdateStatus();
                    };

                    try
                    {
                        if (transfer.IsDirectory)
                        {
                            var downloadTask = FileExplorerResource.DownloadDirectory(transfer.SourcePath,
                                transfer.CancellationToken, _restClient);
                            using (var downloadResponse = await downloadTask)
                            {
                                streamCopyUtil.TotalSize = downloadResponse.Content.Headers.ContentLength ?? 0;

                                var tmpFile = Path.GetTempFileName();
                                using (var localStream = new FileStream(tmpFile, FileMode.Create, FileAccess.ReadWrite,
                                    FileShare.None, 8192, FileOptions.Asynchronous | FileOptions.DeleteOnClose))
                                {
                                    using (var remoteStream = await downloadResponse.Content.ReadAsStreamAsync())
                                    {
                                        transfer.State = FileTransferState.Transferring;
                                        await streamCopyUtil.CopyToAsync(remoteStream, localStream,
                                            transfer.CancellationToken);
                                    }

                                    transfer.State = FileTransferState.Extracting;

                                    localStream.Position = 0;

                                    var targetDirectory = Directory.CreateDirectory(transfer.TargetPath);
                                    using (var zipArchive = new ZipArchive(localStream, ZipArchiveMode.Read, false))
                                    {
                                        streamCopyUtil.Reset();
                                        streamCopyUtil.TotalSize = zipArchive.Entries.Sum(x => x.Length);

                                        string fullName = targetDirectory.FullName;
                                        foreach (var entry in zipArchive.Entries)
                                        {
                                            var entryPath = Path.GetFullPath(Path.Combine(fullName, entry.FullName));
                                            if (Path.GetFileName(entryPath).Length == 0)
                                            {
                                                Directory.CreateDirectory(entryPath);
                                            }
                                            else
                                            {
                                                Directory.CreateDirectory(Path.GetDirectoryName(entryPath));
                                                using (Stream destination = File.Open(entryPath, FileMode.Create, FileAccess.Write, FileShare.None))
                                                {
                                                    using (Stream stream = entry.Open())
                                                        await streamCopyUtil.CopyToAsync(stream, destination,
                                                            transfer.CancellationToken);
                                                }

                                                File.SetLastWriteTime(entryPath, entry.LastWriteTime.DateTime);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            streamCopyUtil.TotalSize = transfer.TotalSize;

                            var downloadTask =
                                FileExplorerResource.DownloadFile(transfer.SourcePath, transfer.CancellationToken, _restClient);

                            using (var downloadResponse = await downloadTask)
                            {
                                using (var localStream = File.Create(transfer.TargetPath))
                                using (var remoteStream = await downloadResponse.Content.ReadAsStreamAsync())
                                {
                                    transfer.State = FileTransferState.Transferring;
                                    await streamCopyUtil.CopyToAsync(remoteStream, localStream, transfer.CancellationToken);
                                }
                            }
                        }

                        await _baseVm.Dispatcher.Current.BeginInvoke(new Action(() =>
                            transfer.CompleteProgress(FileTransferState.Succeeded)));
                    }
                    catch (OperationCanceledException)
                    {
                        await _baseVm.Dispatcher.Current.BeginInvoke(new Action(() =>
                            transfer.CompleteProgress(FileTransferState.Canceled)));
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e, "An error occurred on file download");

                        await _baseVm.Dispatcher.Current.BeginInvoke(new Action(() =>
                            transfer.CompleteProgress(FileTransferState.Failed)));
                    }
                }
                finally
                {
                    _concurrentDownloadsSemaphore.Release();
                }
            }
            finally
            {
                lock (_activeTransfersLock)
                    _activeTransfers.Remove(transfer);
            }

            UpdateStatus();
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

                    var processingEntry = AddProcessingViewModel(transfer);

                    var zipContent = new ZipContent(fs) {CancellationToken = transfer.CancellationToken};
                    zipContent.ProgressChanged += (sender, args) =>
                    {
                        transfer.UpdateProgress(args);

                        processingEntry.Progress = args.Progress;
                        processingEntry.SetSize(args.ProcessedSize);

                        UpdateStatus();
                    };

                    var isRemoved = false;
                    try
                    {
                        transfer.State = FileTransferState.Transferring;
                        await FileExplorerResource.Upload(zipContent, transfer.TargetPath, transfer.CancellationToken,
                            _restClient);

                        await _baseVm.Dispatcher.Current.BeginInvoke(new Action(() =>
                        {
                            transfer.CompleteProgress(FileTransferState.Succeeded);
                            isRemoved = true;
                            _baseVm.ProcessingEntries.Remove(processingEntry);
                            _baseVm.FileSystem.UploadCompleted(processingEntry.Source);
                        }));
                    }
                    catch (OperationCanceledException)
                    {
                        await _baseVm.Dispatcher.Current.BeginInvoke(new Action(() =>
                            transfer.CompleteProgress(FileTransferState.Canceled)));
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e, "An error occurred on file upload");
                        await _baseVm.Dispatcher.Current.BeginInvoke(new Action(() =>
                            transfer.CompleteProgress(FileTransferState.Failed)));
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

        private ProcessingEntryViewModel AddProcessingViewModel(FileTransferViewModel transfer)
        {
            var processingEntry = new ProcessingEntryViewModel(transfer, _baseVm.FileSystem, _baseVm);
            _baseVm.Dispatcher.Current.BeginInvoke(
                new Action(() => _baseVm.ProcessingEntries.Add(processingEntry)),
                DispatcherPriority.ApplicationIdle);

            return processingEntry;
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

                            if (_activeTransfers.Any(x => x.State == FileTransferState.Transferring))
                            {
                                Progress = _activeTransfers.Sum(x => x.Progress ?? 0) / _activeTransfers.Count;
                                _progressStatusMessage.Progress = Progress > 0 ? Progress : (double?) null;

                                TotalSpeed = _activeTransfers.Sum(x => x.CurrentSpeed);
                                _progressStatusMessage.Message =
                                    GetTransmissionStatusMessage() + " " + Tx.DataSize((long) TotalSpeed) + "/s";
                            }
                            else if (_activeTransfers.Any(x => x.State == FileTransferState.Extracting))
                            {
                                Progress = _activeTransfers.Sum(x => x.Progress ?? 0) / _activeTransfers.Count;
                                _progressStatusMessage.Progress = Progress > 0 ? Progress : (double?)null;

                                TotalSpeed = _activeTransfers.Sum(x => x.CurrentSpeed);
                                _progressStatusMessage.Message = Tx.T("FileExplorer:Extracting");
                            }
                            else
                            {
                                _progressStatusMessage.Progress = null;
                                _progressStatusMessage.Message = Tx.T("FileExplorer:StatusBar.PreparingTransmission");
                            }
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