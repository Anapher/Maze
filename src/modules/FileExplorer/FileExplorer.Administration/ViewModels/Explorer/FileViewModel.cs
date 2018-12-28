using System;
using System.Windows.Input;
using System.Windows.Media;
using FileExplorer.Administration.Models;
using FileExplorer.Administration.Models.Args;
using FileExplorer.Administration.Models.Cache;
using FileExplorer.Administration.ViewModels.Explorer.Base;
using FileExplorer.Shared.Dtos;
using Maze.Administration.Library.StatusBar;
using Maze.Utilities;
using Prism.Commands;
using Unclassified.TxLib;

namespace FileExplorer.Administration.ViewModels.Explorer
{
    public class FileViewModel : EntryViewModel
    {
        private readonly FileEntry _fileEntry;
        private readonly IFileSystem _fileSystem;
        private readonly IUiTools _uiTools;
        private Lazy<FileTypeDescription> _lazyFileTypeDescription;
        private bool _isEditingName;
        private ICommand _renameCommand;
        private string _name;

        public FileViewModel(FileEntry fileEntry, IFileSystem fileSystem, IUiTools uiTools)
        {
            _fileSystem = fileSystem;
            _uiTools = uiTools;
            _fileEntry = fileEntry;

            UpdateName();

            fileSystem.EntryUpdated += FileSystemOnEntryUpdated;
        }

        public override string Label => _name;
        public override string Name => _name;
        public override string SortName => _name;
        public override FileExplorerEntry Source => _fileEntry;
        public override bool IsDirectory { get; } = false;
        public override EntryViewModelType Type { get; } = EntryViewModelType.File;
        public override ImageSource Icon => _lazyFileTypeDescription.Value.Icon;
        public override string Description => _lazyFileTypeDescription.Value.Description;
        public override long Size => _fileEntry.Size;

        public override bool IsEditingName
        {
            get => _isEditingName;
            set => SetProperty(ref _isEditingName, value);
        }

        public override ICommand RenameCommand
        {
            get
            {
                return _renameCommand ?? (_renameCommand = new DelegateCommand<string>(
                           parameter =>
                           {
                               _fileSystem.Rename(Source, parameter).DisplayOnStatusBarCatchErrors(_uiTools.StatusBar,
                                   Tx.T("FileExplorer:StatusBar.RenameFile", "name", parameter)).Forget();
                           }, command => _fileSystem.IsValidFilename(command)));
            }
        }

        private void UpdateName()
        {
            if (_name != Source.Name)
            {
                _name = Source.Name;
                _lazyFileTypeDescription =
                    new Lazy<FileTypeDescription>(() => _fileSystem.GetFileTypeDescription(Source.Name));
            }
        }

        private void FileSystemOnEntryUpdated(object sender, EntryUpdatedEventArgs e)
        {
            if (e.Entry == Source)
            {
                UpdateName();

                RaisePropertyChanged(nameof(Name));
                RaisePropertyChanged(nameof(Label));
                RaisePropertyChanged(nameof(SortName));
                RaisePropertyChanged(nameof(Icon));
            }
        }
    }
}