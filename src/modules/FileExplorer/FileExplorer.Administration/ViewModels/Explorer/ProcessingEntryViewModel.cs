using System;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using FileExplorer.Administration.Models;
using FileExplorer.Administration.ViewModels.Explorer.Base;
using FileExplorer.Shared.Dtos;
using Prism.Commands;
using Unclassified.TxLib;

namespace FileExplorer.Administration.ViewModels.Explorer
{
    public class ProcessingEntryViewModel : EntryViewModel
    {
        private readonly IFileSystem _fileSystem;
        private readonly Lazy<ImageSource> _lazyIcon;
        private readonly IUiTools _uiTools;
        private bool _isBusy;
        private double _progress;
        private long _size;

        public ProcessingEntryViewModel(FileTransferViewModel fileTransferViewModel, IFileSystem fileSystem,
            IUiTools uiTools)
        {
            _fileSystem = fileSystem;
            _uiTools = uiTools;
            Label = fileTransferViewModel.Name;
            Name = fileTransferViewModel.Name;
            SortName = fileTransferViewModel.Name;
            IsDirectory = fileTransferViewModel.IsDirectory;
            Description = Tx.T("FileExplorer:Upload");

            if (IsDirectory)
                Source = new DirectoryEntry {HasSubFolder = true};
            else
                Source = new FileEntry {Size = fileTransferViewModel.TotalSize};

            Source.CreationTime = DateTimeOffset.Now;
            Source.LastAccess = DateTimeOffset.Now;
            Source.Name = fileTransferViewModel.Name;
            Source.Path = Path.Combine(fileTransferViewModel.TargetPath, Name);

            _lazyIcon = new Lazy<ImageSource>(CreateIcon);
            CancelCommand = fileTransferViewModel.CancelCommand;
        }

        public override string Label { get; }
        public override string Name { get; }
        public override string SortName { get; }
        public override FileExplorerEntry Source { get; }
        public override bool IsDirectory { get; }
        public override EntryViewModelType Type { get; } = EntryViewModelType.Processing;
        public override ImageSource Icon => _lazyIcon.Value;
        public override string Description { get; }

        public override long Size => _size;

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

        public override bool IsEditingName
        {
            get => false;
            set { }
        }

        public override ICommand RenameCommand { get; } = new DelegateCommand(() => { }, () => false);
        public ICommand CancelCommand { get; }

        public void SetSize(long newSize)
        {
            SetProperty(ref _size, newSize, nameof(Size));
        }

        private ImageSource CreateIcon()
        {
            if (IsDirectory)
                return _uiTools.ImageProvider.GetFolderImage(Name, 0);
            return _fileSystem.GetFileTypeDescription(Source.Path).Icon;
        }
    }
}