using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using FileExplorer.Administration.Models;
using FileExplorer.Administration.ViewModels.Explorer;
using FileExplorer.Administration.ViewModels.Explorer.Base;
using FileExplorer.Shared.Dtos;
using Prism.Mvvm;

namespace FileExplorer.Administration.ViewModels
{
    public class EntriesViewModel : BindableBase
    {
        private readonly FileExplorerViewModel _fileExplorerViewModel;
        private ListCollectionView _view;
        private ObservableCollection<EntryViewModel> _entryViewModels;

        public EntriesViewModel(FileExplorerViewModel fileExplorerViewModel)
        {
            _fileExplorerViewModel = fileExplorerViewModel;
            _fileExplorerViewModel.PathChanged += FileExplorerViewModelOnPathChanged;

            Filters = new List<Predicate<object>>();
            LiveFilteringProperties = new List<string>();

            NavigationBarViewModel = fileExplorerViewModel.NavigationBarViewModel;
        }

        public NavigationBarViewModel NavigationBarViewModel { get; }
        public List<Predicate<object>> Filters { get; }
        public List<string> LiveFilteringProperties { get; }

        public ListCollectionView View
        {
            get => _view;
            private set => SetProperty(ref _view, value);
        }

        private CollectionViewSource _viewSource;

        public CollectionViewSource ViewSource
        {
            get => _viewSource;
            set => SetProperty(ref _viewSource, value);
        }

        private void FileExplorerViewModelOnPathChanged(object sender, PathContent e)
        {
            GenerateEntries(e.Entries, e.Path == "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}");
        }

        private void GenerateEntries(IReadOnlyCollection<FileExplorerEntry> pathEntries, bool isSpecialDirectory)
        {
            List<EntryViewModel> entries;
            if (isSpecialDirectory)
            {
                var counter = 0;
                entries = pathEntries.OfType<DirectoryEntry>()
                    .Select(x => new DirectoryNodeViewModel(x, _fileExplorerViewModel.FileSystem, counter++, _fileExplorerViewModel))
                    .ToList<EntryViewModel>();
            }
            else
                entries = pathEntries.OfType<DirectoryEntry>()
                    .Select(x => new DirectoryNodeViewModel(x, _fileExplorerViewModel.FileSystem, _fileExplorerViewModel))
                    .ToList<EntryViewModel>();

            entries.AddRange(pathEntries.OfType<FileEntry>()
                .Select(x => new FileViewModel(x, _fileExplorerViewModel.FileSystem)));

            _entryViewModels = new ObservableCollection<EntryViewModel>(entries);

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
    }
}