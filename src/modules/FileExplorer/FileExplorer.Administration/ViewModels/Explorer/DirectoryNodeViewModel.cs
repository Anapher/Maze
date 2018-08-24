using System;
using System.Windows.Media;
using FileExplorer.Administration.Models;
using FileExplorer.Administration.ViewModels.Explorer.Base;
using FileExplorer.Shared.Dtos;
using Unclassified.TxLib;

namespace FileExplorer.Administration.ViewModels.Explorer
{
    public class DirectoryNodeViewModel : EntryViewModel
    {
        private readonly IFileSystem _fileSystem;
        private readonly Lazy<string> _lazyLabel;
        private readonly string _sortName;

        public DirectoryNodeViewModel(DirectoryEntry directoryEntry, IFileSystem fileSystem, int orderNumber) : this(
            directoryEntry, fileSystem)
        {
            _sortName = orderNumber.ToString("0000");
        }

        public DirectoryNodeViewModel(DirectoryEntry directoryEntry, IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            Source = directoryEntry;

            if (directoryEntry is DriveDirectoryEntry driveDirectory)
            {
                Size = driveDirectory.UsedSpace;
                Description = Tx.T($"FileExplorer:DriveTypes.{driveDirectory.DriveType}");
            }
            else
            {
                Description = Tx.T("FileExplorer:Directory");
            }

            _lazyLabel = new Lazy<string>(CreateLabel);

            Name = directoryEntry.Name;
        }

        public override string Label => _lazyLabel.Value;
        public override string Name { get; set; }
        public override string SortName => _sortName ?? Label;
        public override FileExplorerEntry Source { get; }
        public override bool IsDirectory { get; } = true;
        public override EntryViewModelType Type { get; } = EntryViewModelType.Directory;
        public override ImageSource Icon { get; }
        public override string Description { get; }
        public override long Size { get; }

        private string CreateLabel()
        {
            if (Source is SpecialDirectoryEntry specialDirectory)
            {
                if (specialDirectory.LabelId != 0 && !string.IsNullOrEmpty(specialDirectory.LabelPath))
                {
                    var label = _fileSystem.GetLabel(specialDirectory.LabelPath, specialDirectory.LabelId);
                    if (!string.IsNullOrEmpty(label))
                        return label;
                }

                if (!string.IsNullOrEmpty(specialDirectory.Label))
                    return specialDirectory.Label;
            }

            return Source.Name;
        }
    }
}