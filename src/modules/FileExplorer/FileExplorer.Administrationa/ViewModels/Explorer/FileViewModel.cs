using System;
using System.Windows.Media;
using FileExplorer.Administration.Models;
using FileExplorer.Administration.Utilities;
using FileExplorer.Administration.ViewModels.Explorer.Base;
using FileExplorer.Shared.Dtos;

namespace FileExplorer.Administration.ViewModels.Explorer
{
    public class FileViewModel : EntryViewModel
    {
        private readonly FileEntry _fileEntry;
        private readonly IFileSystem _fileSystem;
        private readonly Lazy<FileTypeDescription> _lazyFileTypeDescription;

        public FileViewModel(FileEntry fileEntry, IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _fileEntry = fileEntry;

            Name = fileEntry.Name;

            _lazyFileTypeDescription =
                new Lazy<FileTypeDescription>(() => fileSystem.GetFileTypeDescription(fileEntry.Name));
        }

        public override string Label => Name;
        public override string Name { get; set; }
        public override string SortName => Label;
        public override FileExplorerEntry Source => _fileEntry;
        public override bool IsDirectory { get; } = false;
        public override EntryViewModelType Type { get; } = EntryViewModelType.File;
        public override ImageSource Icon => _lazyFileTypeDescription.Value.Icon;
        public override string Description => _lazyFileTypeDescription.Value.Description;
        public override long Size => _fileEntry.Size;
    }
}