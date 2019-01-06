using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Anapher.Wpf.Toolkit;
using Anapher.Wpf.Toolkit.Extensions;
using Anapher.Wpf.Toolkit.StatusBar;
using Anapher.Wpf.Toolkit.Utilities;
using FileExplorer.Administration.Models;
using FileExplorer.Administration.ViewModels.Explorer;
using FileExplorer.Administration.ViewModels.Explorer.Base;
using FileExplorer.Shared.Dtos;
using Maze.Utilities;
using Prism.Commands;
using Prism.Mvvm;
using Unclassified.TxLib;

namespace FileExplorer.Administration.ViewModels
{
    public class EntriesViewModel : BindableBase, IFileExplorerChildViewModel
    {
        private readonly TransactionalObservableCollection<EntryViewModel> _entryViewModels;
        private DelegateCommand<EntryViewModel> _enterNameEditingCommand;
        private FileExplorerViewModel _fileExplorerViewModel;
        private AsyncDelegateCommand<DirectoryViewModel> _openDirectoryCommand;
        private DelegateCommand<IList> _removeEntriesCommand;
        private ListCollectionView _view;
        private DelegateCommand _refreshCommand;

        public EntriesViewModel()
        {
            Filters = new List<Predicate<object>>();
            LiveFilteringProperties = new List<string>();

            _entryViewModels = new TransactionalObservableCollection<EntryViewModel>();
        }

        public NavigationBarViewModel NavigationBarViewModel => _fileExplorerViewModel.NavigationBarViewModel;
        public List<Predicate<object>> Filters { get; }
        public List<string> LiveFilteringProperties { get; }

        public ListCollectionView View
        {
            get => _view;
            private set => SetProperty(ref _view, value);
        }

        public AsyncDelegateCommand<DirectoryViewModel> OpenDirectoryCommand
        {
            get
            {
                return _openDirectoryCommand ?? (_openDirectoryCommand =
                           new AsyncDelegateCommand<DirectoryViewModel>(parameter =>
                               _fileExplorerViewModel.OpenPath(parameter.Source.Path)));
            }
        }

        public DelegateCommand<IList> RemoveEntriesCommand
        {
            get
            {
                return _removeEntriesCommand ?? (_removeEntriesCommand = new DelegateCommand<IList>(parameter =>
                {
                    RemoveEntries(parameter.Cast<EntryViewModel>()).Forget();
                }));
            }
        }

        public DelegateCommand<EntryViewModel> EnterNameEditingCommand
        {
            get
            {
                return _enterNameEditingCommand ?? (_enterNameEditingCommand =
                           new DelegateCommand<EntryViewModel>(parameter => { parameter.IsEditingName = true; }));
            }
        }

        public DelegateCommand RefreshCommand
        {
            get
            {
                return _refreshCommand ?? (_refreshCommand = new DelegateCommand(() =>
                {
                    _fileExplorerViewModel.OpenPath(_fileExplorerViewModel.CurrentPath, true).Forget();
                }));
            }
        }

        public void Initialize(FileExplorerViewModel fileExplorerViewModel)
        {
            _fileExplorerViewModel = fileExplorerViewModel;
            _fileExplorerViewModel.PathChanged += FileExplorerViewModelOnPathChanged;
            _fileExplorerViewModel.FileSystem.EntryAdded += FileSystemOnEntryAdded;
            _fileExplorerViewModel.FileSystem.EntryRemoved += FileSystemOnEntryRemoved;
            _fileExplorerViewModel.ProcessingEntries.CollectionChanged += ProcessingEntriesOnCollectionChanged;

            View = new ListCollectionView(_entryViewModels);

            foreach (var predicate in Filters)
                View.Filter += predicate;

            View.SortDescriptions.Add(new SortDescription(nameof(EntryViewModel.IsDirectory),
                ListSortDirection.Descending));
            View.SortDescriptions.Add(new SortDescription(nameof(EntryViewModel.SortName),
                ListSortDirection.Ascending));
            View.LiveSortingProperties.Add(nameof(EntryViewModel.SortName));
            View.IsLiveSorting = true;

            View.LiveFilteringProperties.AddRange(LiveFilteringProperties);
            View.IsLiveFiltering = true;
        }

        public async Task RemoveEntries(IEnumerable<EntryViewModel> entries)
        {
            var directories = new List<DirectoryViewModel>();
            var files = new List<FileViewModel>();

            foreach (var entryViewModel in entries)
                if (entryViewModel is DirectoryViewModel directory)
                    directories.Add(directory);
                else if (entryViewModel is FileViewModel file)
                    files.Add(file);

            if (!files.Any() && !directories.Any())
                return;

            string filesText;
            string directoriesText;

            if (files.Count == 1)
                filesText = Tx.T("FileExplorer:Remove.Files", 1, "name", files.Single().Label);
            else if (files.Any())
                filesText = Tx.T("FileExplorer:Remove.Files", files.Count);
            else filesText = null;

            if (directories.Count == 1)
                directoriesText = Tx.T("FileExplorer:Remove.Directories", 1, "name", directories.Single().Label);
            else if (directories.Any())
                directoriesText = Tx.T("FileExplorer:Remove.Directories", directories.Count);
            else directoriesText = null;

            string message;
            if (filesText == null || directoriesText == null)
                message = Tx.T("FileExplorer:Remove.Prompt", "entities", filesText ?? directoriesText);
            else
                message = Tx.T("FileExplorer:Remove.PromptMultiple", "entities1", filesText, "entities2",
                    directoriesText);

            if (_fileExplorerViewModel.Window.ShowMessage(message, Tx.T("Warning"), MessageBoxButton.OKCancel,
                    MessageBoxImage.Warning) != MessageBoxResult.OK)
                return;

            var entriesToDelete = files.Cast<EntryViewModel>().Concat(directories).ToList();
            var statusMessage = Tx.T("FileExplorer:StatusBar.DeleteEntries", entriesToDelete.Count);

            var progress = new ProgressStatusMessage(statusMessage);
            using (_fileExplorerViewModel.StatusBar.PushStatus(progress))
            {
                var entriesCounter = 0;

                void IncrementProgress()
                {
                    double processed = Interlocked.Increment(ref entriesCounter);
                    progress.Progress = processed / entriesToDelete.Count;
                }

                var errors = await TaskCombinators.ThrottledCatchErrorsAsync(entriesToDelete,
                    (model, token) =>
                    {
                        return _fileExplorerViewModel.FileSystem.Remove(model.Source)
                            .ContinueWith(task => IncrementProgress());
                    }, CancellationToken.None);

                if (errors.Any()) _fileExplorerViewModel.StatusBar.ShowWarning("Failed to delete some entries");
            }
        }

        private void FileExplorerViewModelOnPathChanged(object sender, PathContent e)
        {
            GenerateEntries(e.Entries, e.Path == "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}");
        }

        private void FileSystemOnEntryRemoved(object sender, FileExplorerEntry e)
        {
            var parentFolder = Path.GetDirectoryName(e.Path);
            if (_fileExplorerViewModel.FileSystem.ComparePaths(_fileExplorerViewModel.CurrentPath, parentFolder))
                _fileExplorerViewModel.Dispatcher.Current.InvokeIfRequired(() =>
                {
                    var viewModel = _entryViewModels.FirstOrDefault(x => x.Source.Path == e.Path);
                    if (viewModel != null)
                        _entryViewModels.Remove(viewModel);
                });
        }

        private void FileSystemOnEntryAdded(object sender, FileExplorerEntry e)
        {
            if (_fileExplorerViewModel.FileSystem.ComparePaths(_fileExplorerViewModel.CurrentPath, e.Parent?.Path))
                _fileExplorerViewModel.Dispatcher.Current.InvokeIfRequired(() =>
                {
                    if (_entryViewModels.Any(x => string.Equals(x.Name, e.Name, StringComparison.OrdinalIgnoreCase)))
                        return;

                    if (e is DirectoryEntry directory)
                        _entryViewModels.Add(new DirectoryViewModel(directory, _fileExplorerViewModel.FileSystem,
                            _fileExplorerViewModel, false));
                    else if (e is FileEntry file)
                        _entryViewModels.Add(new FileViewModel(file, _fileExplorerViewModel.FileSystem, _fileExplorerViewModel));
                });
        }

        private void ProcessingEntriesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var entry in e.NewItems.Cast<ProcessingEntryViewModel>())
                {
                    AddProcessingEntryIfPathMatches(entry);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var entry in e.OldItems.Cast<ProcessingEntryViewModel>())
                {
                    _entryViewModels.Remove(entry);
                }
            }
        }

        private void GenerateEntries(IReadOnlyCollection<FileExplorerEntry> pathEntries, bool isSpecialDirectory)
        {
            if (View.IsEditingItem)
                View.CancelEdit();

            IEnumerable<EntryViewModel> entries;
            if (isSpecialDirectory)
            {
                var counter = 0;
                entries = pathEntries.OfType<DirectoryEntry>().Select(x =>
                    new DirectoryViewModel(x, _fileExplorerViewModel.FileSystem, counter++, _fileExplorerViewModel));
            }
            else
            {
                entries = pathEntries.OfType<DirectoryEntry>().Select(x =>
                    new DirectoryViewModel(x, _fileExplorerViewModel.FileSystem, _fileExplorerViewModel, false));
            }

            entries = entries.Concat(pathEntries.OfType<FileEntry>()
                .Select(x => new FileViewModel(x, _fileExplorerViewModel.FileSystem, _fileExplorerViewModel)));

            _entryViewModels.SuspendCollectionChangeNotification();
            try
            {
                _entryViewModels.Clear();

                foreach (var entryViewModel in entries)
                    _entryViewModels.Add(entryViewModel);

                foreach (var entry in _fileExplorerViewModel.ProcessingEntries)
                    AddProcessingEntryIfPathMatches(entry);
            }
            finally
            {
                _entryViewModels.NotifyChanges();
            }
        }

        private void AddProcessingEntryIfPathMatches(EntryViewModel entry)
        {
            if (_fileExplorerViewModel.FileSystem.ComparePaths(_fileExplorerViewModel.CurrentPath,
                Path.GetDirectoryName(entry.Source.Path)))
            {
                _entryViewModels.Add(entry);
            }
        }
    }
}