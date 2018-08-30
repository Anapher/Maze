using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using FileExplorer.Administration.Controls;
using FileExplorer.Administration.Controls.Models;
using FileExplorer.Administration.Helpers;
using FileExplorer.Administration.Models;
using FileExplorer.Administration.Utilities;
using FileExplorer.Administration.ViewModels.Explorer.Base;
using FileExplorer.Shared.Dtos;
using Orcus.Administration.Library.Extensions;
using Orcus.Administration.Library.StatusBar;
using Orcus.Administration.Library.Utilities;
using Orcus.Utilities;
using Prism.Commands;
using Unclassified.TxLib;

namespace FileExplorer.Administration.ViewModels.Explorer
{
    public class DirectoryViewModel : EntryViewModel,
        ISupportTreeSelector<DirectoryViewModel, FileExplorerEntry>, IIntoViewBringable, IEquatable<EntryViewModel>,
        IAsyncAutoComplete
    {
        private readonly IFileSystem _fileSystem;
        private Lazy<ImageSource> _lazyIcon;
        private Lazy<string> _lazyLabel;
        private readonly DirectoryTreeViewModel _rootViewModel;
        private readonly string _sortName;
        private readonly DirectoryEntry _source;
        private readonly IUiTools _uiTools;
        private bool _isBreadcrumbExpanded;
        private int _bringIntoViewToken;
        private bool _isLoadingEntries;
        private bool _isEditingName;
        private ICommand _renameCommand;
        private readonly bool _isNodeViewModel;
        private string _name;

        public DirectoryViewModel(DirectoryEntry directoryEntry, IFileSystem fileSystem, int orderNumber,
            IUiTools uiTools) : this(directoryEntry, fileSystem, uiTools, false)
        {
            _sortName = orderNumber.ToString("0000");
        }

        public DirectoryViewModel(DirectoryTreeViewModel rootViewModel, DirectoryViewModel parentViewModel,
            DirectoryEntry directoryEntry, IFileSystem fileSystem, IUiTools uiTools) : this(directoryEntry, fileSystem,
            uiTools, true)
        {
            _rootViewModel = rootViewModel;
            Parent = parentViewModel;

            Entries = new EntriesHelper<DirectoryViewModel>(LoadEntriesAsync);
            Selection = new TreeSelector<DirectoryViewModel, FileExplorerEntry>(Source, this,
                parentViewModel?.Selection ?? rootViewModel.Selection, Entries);

            if (!directoryEntry.HasSubFolder)
                Entries.SetEntries(UpdateMode.Update);
        }

        public DirectoryViewModel(DirectoryEntry directoryEntry, IFileSystem fileSystem, IUiTools uiTools, bool isNode)
        {
            _fileSystem = fileSystem;
            _uiTools = uiTools;
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

            UpdateName();

            if (isNode)
            {
                fileSystem.EntryAdded += FileSystemOnEntryAdded;
                fileSystem.EntryRemoved += FileSystemOnEntryRemoved;
                fileSystem.DirectoryEntriesUpdated += FileSystemOnDirectoryEntriesUpdated;
            }

            fileSystem.EntryUpdated += FileSystemOnEntryUpdated;
            _isNodeViewModel = isNode;
        }

        public override string Label => _lazyLabel.Value;
        public override string Name => _name;
        public override string SortName => _sortName ?? Label;
        public override FileExplorerEntry Source => _source;
        public override bool IsDirectory { get; } = true;
        public override EntryViewModelType Type { get; } = EntryViewModelType.Directory;
        public override ImageSource Icon => _lazyIcon.Value;
        public override string Description { get; }
        public override long Size { get; }

        public IEntriesHelper<DirectoryViewModel> Entries { get; set; }
        public ITreeSelector<DirectoryViewModel, FileExplorerEntry> Selection { get; set; }
        public DirectoryViewModel Parent { get; }

        public int BringIntoViewToken
        {
            get => _bringIntoViewToken;
            set => SetProperty(ref _bringIntoViewToken, value);
        }

        public bool IsBreadcrumbExpanded
        {
            get => _isBreadcrumbExpanded;
            set
            {
                if (_isBreadcrumbExpanded != value)
                {
                    if (value)
                        Entries.LoadAsync();

                    _isBreadcrumbExpanded = value;
                    //no need to notify property changed
                }
            }
        }

        public override bool IsEditingName
        {
            get => _isEditingName;
            set => SetProperty(ref _isEditingName, value);
        }

        public override ICommand RenameCommand
        {
            get
            {
                return _renameCommand ?? (_renameCommand = new DelegateCommand<string>(parameter =>
                {
                    if (Source.Type == FileExplorerEntryType.Drive)
                        return;

                    if (((DirectoryEntry) Source).IsComputerDirectory())
                        return;

                    _fileSystem.Rename(Source, parameter).DisplayOnStatusBarCatchErrors(_uiTools.StatusBar,
                        Tx.T("FileExplorer:StatusBar.RenameDirectory", "name", parameter)).Forget();
                }, command => _fileSystem.IsValidFilename(command)));
            }
        }

        public void BringIntoView()
        {
            BringIntoViewToken++;
        }

        public async ValueTask<IEnumerable> GetAutoCompleteEntries()
        {
            if (!Entries.IsLoaded)
            {
                return await Entries.LoadAsync(UpdateMode.Replace, false, null, await _uiTools.Dispatcher.GetTaskScheduler());
            }

            return Entries.All;
        }

        public void UpdateName()
        {
            if (_name != Source.Name)
            {
                _name = Source.Name;
                _lazyLabel = new Lazy<string>(CreateLabel);
                _lazyIcon = new Lazy<ImageSource>(CreateIcon);
            }
        }

        private string CreateLabel()
        {
            if (_source is SpecialDirectoryEntry specialDirectory)
                if (specialDirectory.LabelId != 0 && !string.IsNullOrEmpty(specialDirectory.LabelPath))
                {
                    var label = _fileSystem.GetLabel(specialDirectory.LabelPath, specialDirectory.LabelId);
                    if (!string.IsNullOrEmpty(label))
                        return label;
                }

            if (!string.IsNullOrEmpty(_source.Label))
                return _source.Label;

            return Source.Name;
        }

        private ImageSource CreateIcon()
        {
            if (_source is SpecialDirectoryEntry specialDirectory)
                return _uiTools.ImageProvider.GetFolderImage(specialDirectory);

            return _uiTools.ImageProvider.GetFolderImage(_source.Name, 0);
        }

        private async Task<IEnumerable<DirectoryViewModel>> LoadEntriesAsync()
        {
            if (!_source.HasSubFolder)
                return Enumerable.Empty<DirectoryViewModel>();

            _isLoadingEntries = true;
            try
            {
                var result = await _fileSystem.FetchSubDirectories(_source, false)
                    .DisplayOnStatusBarCatchErrors(_uiTools.StatusBar, Tx.T("FileExplorer:StatusBar.LoadSubDirectories"),
                        StatusBarAnimation.Search);
                if (result.Failed)
                    return Enumerable.Empty<DirectoryViewModel>();

                var viewModels = result.Result.Select(CreateDirectoryViewModel);
                if (!_source.IsComputerDirectory())
                    viewModels = viewModels.OrderBy(x => x.Label);

                return viewModels;
            }
            finally
            {
                _isLoadingEntries = false;
            }
        }

        private void FileSystemOnEntryUpdated(object sender, EntryUpdatedEventArgs e)
        {
            if (e.Entry.Type == FileExplorerEntryType.File)
                return;

            if (e.Entry == Source)
            {
                //the entry that was updated is actually this one
                UpdateName();
                RaisePropertyChanged(nameof(Name));
                RaisePropertyChanged(nameof(Label));
                RaisePropertyChanged(nameof(Icon));
                RaisePropertyChanged(nameof(SortName));
            }
            else if (_isNodeViewModel && Entries.IsLoaded)
            {
                if (_fileSystem.ComparePaths(Source.Path, e.OldParentPath))
                {
                    //the entry was previously in this directory
                    _uiTools.Dispatcher.Current.InvokeIfRequired(() =>
                    {
                        var newParentPath = Path.GetDirectoryName(e.Entry.Path);
                        if (_fileSystem.ComparePaths(newParentPath, Source.Path))
                        {
                            //only the name changed, just reorder
                            var viewModel = Entries.All.FirstOrDefault(x => x.Source == e.Entry);
                            if (viewModel != null)
                            {
                                viewModel.UpdateName(); //important if the entry didn't receive the event first
                                Entries.All.Remove(viewModel);
                                Entries.All.AddSorted(viewModel, x => x.Label);
                            }
                        }
                        else
                        {
                            //remove as the path changed to a different parent folder
                            var viewModel = Entries.All.FirstOrDefault(x => x.Source == e.Entry);
                            if (viewModel != null)
                            {
                                if (viewModel.Selection.IsSelected)
                                {
                                    viewModel.Selection.IsSelected = false;
                                    Selection.IsSelected = true;
                                }

                                Entries.All.Remove(viewModel);
                            }
                        }
                    });
                }
                else if (_fileSystem.ComparePaths(Source.Path, Path.GetDirectoryName(e.Entry.Path)))
                {
                    //the entry was not in this directory but was moved to it
                    _uiTools.Dispatcher.Current.InvokeIfRequired(() =>
                        {
                            Entries.All.AddSorted(CreateDirectoryViewModel((DirectoryEntry) e.Entry), x => x.Label);
                        });
                }
            }
        }

        private void FileSystemOnDirectoryEntriesUpdated(object sender, DirectoryEntriesUpdatedEventArgs e)
        {
            if (_isLoadingEntries || !_fileSystem.ComparePaths(Source.Path, e.DirectoryPath))
                return;

            var directories = e.Entries.OfType<DirectoryEntry>();
            var viewModels = directories.Select(CreateDirectoryViewModel);
            if (!_source.IsComputerDirectory())
                viewModels = viewModels.OrderBy(x => x.Label);

            _uiTools.Dispatcher.Current.InvokeIfRequired(() =>
            {
                Entries.SetEntries(UpdateMode.Update, viewModels.ToArray());
                Entries.IsLoaded = true;
            });
        }

        private void FileSystemOnEntryAdded(object sender, FileExplorerEntry e)
        {
            if (e.Type != FileExplorerEntryType.File && Entries.IsLoaded && e.Parent == Source)
            {
                _uiTools.Dispatcher.Current.InvokeIfRequired(() =>
                {
                    Entries.All.AddSorted(CreateDirectoryViewModel((DirectoryEntry) e), x => x.Label);
                });
            }
        }

        private void FileSystemOnEntryRemoved(object sender, FileExplorerEntry e)
        {
            if (Entries.IsLoaded && e.Type != FileExplorerEntryType.File && e.Parent == Source)
            {
                _uiTools.Dispatcher.Current.InvokeIfRequired(() =>
                {
                    var viewModel = Entries.All.FirstOrDefault(x => x.Source == e);
                    if (viewModel != null)
                    {
                        if (viewModel.Selection.IsSelected)
                        {
                            viewModel.Selection.IsSelected = false;
                            Selection.IsSelected = true;
                        }

                        Entries.All.Remove(viewModel);
                    }
                });
            }
        }

        private DirectoryViewModel CreateDirectoryViewModel(DirectoryEntry directoryEntry) =>
            new DirectoryViewModel(_rootViewModel, this, directoryEntry, _fileSystem, _uiTools);

        public bool Equals(EntryViewModel other) => Equals((object)other);

        protected bool Equals(DirectoryViewModel other) =>
            Equals(Source, other.Source) && string.Equals(Description, other.Description) && Size == other.Size;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(DirectoryViewModel)) return false;
            return Equals((DirectoryViewModel) obj);
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