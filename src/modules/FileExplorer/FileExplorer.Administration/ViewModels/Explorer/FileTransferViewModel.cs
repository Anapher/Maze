using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using FileExplorer.Administration.Models;
using FileExplorer.Administration.ViewModels.Explorer.Base;
using Prism.Commands;
using Prism.Mvvm;

namespace FileExplorer.Administration.ViewModels.Explorer
{
    public class FileTransferViewModel : BindableBase
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private DelegateCommand _cancelCommand;
        private double _currentSpeed;
        private TimeSpan _estimatedRemainingTime;
        private DelegateCommand _openFolderCommand;
        private long _processedSize;
        private double _progress;
        private FileTransferState _state;
        private long _totalSize;

        public FileTransferViewModel(EntryViewModel entryViewModel, string targetPath)
        {
            Name = entryViewModel.Name;
            IsDirectory = entryViewModel.IsDirectory;
            IsUpload = false; //download
            SourcePath = entryViewModel.Source.Path;
            TargetPath = targetPath;

            if (entryViewModel is FileViewModel file)
                TotalSize = file.Size;
        }

        public FileTransferViewModel(FileSystemInfo fileSystemInfo, string targetPath)
        {
            Name = fileSystemInfo.Name;
            IsUpload = true;
            SourcePath = fileSystemInfo.FullName;
            TargetPath = targetPath;

            if (fileSystemInfo is FileInfo file)
                TotalSize = file.Length;
            else IsDirectory = true;
        }

        public string Name { get; }
        public bool IsDirectory { get; }
        public bool IsUpload { get; }
        public string SourcePath { get; }
        public string TargetPath { get; }
        public CancellationToken CancellationToken => _cancellationTokenSource.Token;
        public DateTimeOffset Timestamp { get; } = DateTimeOffset.Now;

        public long TotalSize
        {
            get => _totalSize;
            set => SetProperty(ref _totalSize, value);
        }

        public long ProcessedSize
        {
            get => _processedSize;
            set => SetProperty(ref _processedSize, value);
        }

        public double Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        public double CurrentSpeed
        {
            get => _currentSpeed;
            set => SetProperty(ref _currentSpeed, value);
        }

        public TimeSpan EstimatedRemainingTime
        {
            get => _estimatedRemainingTime;
            set => SetProperty(ref _estimatedRemainingTime, value);
        }

        public FileTransferState State
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }

        public DelegateCommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand =
                           new DelegateCommand(() => _cancellationTokenSource.Cancel(),
                               () => State <= FileTransferState.Transferring).ObservesProperty(() => State));
            }
        }

        public DelegateCommand OpenFolderCommand
        {
            get
            {
                return _openFolderCommand ?? (_openFolderCommand =
                           new DelegateCommand(
                               () =>
                               {
                                   Process.Start("explorer.exe",
                                       $"/select, \"{(IsUpload ? SourcePath : TargetPath)}\"");
                               }, () => State == FileTransferState.Succeeded).ObservesProperty(() => State));
            }
        }

        public void UpdateProgress(TransferProgressChangedEventArgs args)
        {
            State = FileTransferState.Transferring;

            Progress = args.Progress;
            TotalSize = args.TotalSize;
            ProcessedSize = args.ProcessedSize;
            CurrentSpeed = args.Speed;
            EstimatedRemainingTime = args.EstimatedTime;
        }

        public void CompleteProgress(FileTransferState state)
        {
            State = state;

            Progress = 1;
            ProcessedSize = TotalSize;
            CurrentSpeed = 0;
            EstimatedRemainingTime = TimeSpan.Zero;
        }
    }
}