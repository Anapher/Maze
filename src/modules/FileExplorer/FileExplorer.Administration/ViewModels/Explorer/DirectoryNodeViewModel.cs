using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using FileExplorer.Administration.Controls.Models;
using FileExplorer.Administration.Helpers;
using FileExplorer.Administration.Models;
using FileExplorer.Administration.ViewModels.Explorer.Base;
using FileExplorer.Shared.Dtos;
using Unclassified.TxLib;

namespace FileExplorer.Administration.ViewModels.Explorer
{
    public class DirectoryNodeViewModel : EntryViewModel,
        ISupportTreeSelector<DirectoryNodeViewModel, FileExplorerEntry>, IIntoViewBringable, IEquatable<EntryViewModel>
    {
        private readonly DirectoryTreeViewModel _rootViewModel;
        private readonly IFileSystem _fileSystem;
        private readonly Lazy<string> _lazyLabel;
        private readonly string _sortName;
        private readonly DirectoryEntry _source;
        private bool _isBringIntoView;

        public DirectoryNodeViewModel(DirectoryEntry directoryEntry, IFileSystem fileSystem, int orderNumber) : this(
            directoryEntry, fileSystem)
        {
            _sortName = orderNumber.ToString("0000");
        }

        public DirectoryNodeViewModel(DirectoryTreeViewModel rootViewModel, DirectoryNodeViewModel parentViewModel,
            DirectoryEntry directoryEntry, IFileSystem fileSystem) : this(directoryEntry, fileSystem)
        {
            _rootViewModel = rootViewModel;
            Parent = parentViewModel;

            Entries = new EntriesHelper<DirectoryNodeViewModel>(LoadEntriesAsync);
            Selection = new TreeSelector<DirectoryNodeViewModel, FileExplorerEntry>(Source, this,
                parentViewModel?.Selection ?? rootViewModel.Selection, Entries);

            if (!directoryEntry.HasSubFolder)
                Entries.SetEntries(UpdateMode.Update);
        }

        public DirectoryNodeViewModel(DirectoryEntry directoryEntry, IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _source = directoryEntry;

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
        public override FileExplorerEntry Source => _source;
        public override bool IsDirectory { get; } = true;
        public override EntryViewModelType Type { get; } = EntryViewModelType.Directory;
        public override ImageSource Icon { get; }
        public override string Description { get; }
        public override long Size { get; }

        public DirectoryNodeViewModel Parent { get; }
        public IEntriesHelper<DirectoryNodeViewModel> Entries { get; set; }
        public ITreeSelector<DirectoryNodeViewModel, FileExplorerEntry> Selection { get; set; }

        public bool IsBringIntoView
        {
            get => _isBringIntoView;
            set => SetProperty(ref _isBringIntoView, value);
        }

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

        private async Task<IEnumerable<DirectoryNodeViewModel>> LoadEntriesAsync()
        {
            if (!_source.HasSubFolder)
                return Enumerable.Empty<DirectoryNodeViewModel>();

            var entries = await _fileSystem.FetchSubDirectories(_source, false);
            return entries.Select(CreateDirectoryViewModel);
        }

        private DirectoryNodeViewModel CreateDirectoryViewModel(DirectoryEntry directoryEntry)
        {
            return new DirectoryNodeViewModel(_rootViewModel, this, directoryEntry, _fileSystem);
        }

        public bool Equals(EntryViewModel other) => Equals((object) other);

        protected bool Equals(DirectoryNodeViewModel other) =>
            Equals(Source, other.Source) && string.Equals(Description, other.Description) && Size == other.Size;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(DirectoryNodeViewModel)) return false;
            return Equals((DirectoryNodeViewModel) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Source != null ? Source.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Size.GetHashCode();
                return hashCode;
            }
        }
    }
}