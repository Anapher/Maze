using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Anapher.Wpf.Swan;
using FileExplorer.Administration.Models;
using FileExplorer.Administration.ViewModels.Explorer;
using FileExplorer.Administration.ViewModels.Explorer.Base;
using FileExplorer.Shared.Dtos;
using Prism.Mvvm;

namespace FileExplorer.Administration.ViewModels
{
    public class EntriesViewModel : BindableBase, IFileExplorerChildViewModel
    {
        private FileExplorerViewModel _fileExplorerViewModel;
        private ListCollectionView _view;
        private readonly TransactionalObservableCollection<EntryViewModel> _entryViewModels;
        private AsyncRelayCommand<DirectoryNodeViewModel> _openDirectoryCommand;

        public EntriesViewModel()
        {
            Filters = new List<Predicate<object>>();
            LiveFilteringProperties = new List<string>();

            _entryViewModels = new TransactionalObservableCollection<EntryViewModel>();
        }

        public void Initialize(FileExplorerViewModel fileExplorerViewModel)
        {
            _fileExplorerViewModel = fileExplorerViewModel;
            _fileExplorerViewModel.PathChanged += FileExplorerViewModelOnPathChanged;

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

        public NavigationBarViewModel NavigationBarViewModel => _fileExplorerViewModel.NavigationBarViewModel;
        public List<Predicate<object>> Filters { get; }
        public List<string> LiveFilteringProperties { get; }

        public ListCollectionView View
        {
            get => _view;
            private set => SetProperty(ref _view, value);
        }

        public AsyncRelayCommand<DirectoryNodeViewModel> OpenDirectoryCommand
        {
            get
            {
                return _openDirectoryCommand ?? (_openDirectoryCommand =
                           new AsyncRelayCommand<DirectoryNodeViewModel>(parameter => _fileExplorerViewModel.OpenPath(parameter.Source.Path)));
            }
        }

        private void FileExplorerViewModelOnPathChanged(object sender, PathContent e)
        {
            GenerateEntries(e.Entries, e.Path == "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}");
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
                    new DirectoryNodeViewModel(x, _fileExplorerViewModel.FileSystem, counter++,
                        _fileExplorerViewModel));
            }
            else
                entries = pathEntries.OfType<DirectoryEntry>().Select(x =>
                    new DirectoryNodeViewModel(x, _fileExplorerViewModel.FileSystem, _fileExplorerViewModel));

            entries = entries.Concat(pathEntries.OfType<FileEntry>()
                .Select(x => new FileViewModel(x, _fileExplorerViewModel.FileSystem)));
            
            _entryViewModels.SuspendCollectionChangeNotification();
            try
            {
                _entryViewModels.Clear();
                foreach (var entryViewModel in entries)
                    _entryViewModels.Add(entryViewModel);
            }
            finally
            {
                _entryViewModels.NotifyChanges();
            }
        }
    }
}