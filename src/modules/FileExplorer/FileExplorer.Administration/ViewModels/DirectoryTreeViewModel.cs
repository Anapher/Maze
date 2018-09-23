using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using FileExplorer.Administration.Models;
using FileExplorer.Administration.Utilities;
using FileExplorer.Administration.ViewModels.Explorer;
using FileExplorer.Administration.ViewModels.Explorer.Helpers;
using FileExplorer.Shared.Dtos;
using Orcus.Utilities;
using Prism.Mvvm;
using TreeViewEx.Controls;
using TreeViewEx.Controls.Models;
using TreeViewEx.Extensions;
using TreeViewEx.Helpers;
using TreeViewEx.Helpers.Selectors;
using TreeViewEx.Helpers.Selectors.Lookup;
using TreeViewEx.Helpers.Selectors.Processors;

namespace FileExplorer.Administration.ViewModels
{
    public class DirectoryTreeViewModel : BindableBase, ISupportTreeSelector<DirectoryViewModel, FileExplorerEntry>,
        IAsyncAutoComplete, IFileExplorerChildViewModel
    {
        private FileExplorerViewModel _fileExplorerViewModel;
        private ObservableCollection<DirectoryViewModel> _autoCompleteEntries;
        private List<DirectoryViewModel> _rootViewModels;
        private DirectoryViewModel _selectedViewModel;
        private FileExplorerPathComparer _fileExplorerPathComparer;

        public void Initialize(FileExplorerViewModel fileExplorerViewModel)
        {
            _fileExplorerViewModel = fileExplorerViewModel;

            Entries = new EntriesHelper<DirectoryViewModel>();
            Selection = new TreeRootSelector<DirectoryViewModel, FileExplorerEntry>(Entries)
            {
                Comparers = new[]
                    {_fileExplorerPathComparer = new FileExplorerPathComparer(fileExplorerViewModel.FileSystem)}
            };

            Selection.AsRoot().SelectionChanged += OnSelectionChanged;
            _fileExplorerViewModel.PathChanged += FileExplorerViewModelOnPathChanged;
        }

        public NavigationBarViewModel NavigationBarViewModel => _fileExplorerViewModel.NavigationBarViewModel;

        public List<DirectoryViewModel> RootViewModels
        {
            get => _rootViewModels;
            private set => SetProperty(ref _rootViewModels, value);
        }

        public ObservableCollection<DirectoryViewModel> AutoCompleteEntries
        {
            get => _autoCompleteEntries;
            private set => SetProperty(ref _autoCompleteEntries, value);
        }

        public DirectoryViewModel SelectedViewModel
        {
            get => _selectedViewModel;
            private set => SetProperty(ref _selectedViewModel, value);
        }

        public IEntriesHelper<DirectoryViewModel> Entries { get; set; }
        public ITreeSelector<DirectoryViewModel, FileExplorerEntry> Selection { get; set; }

        public Task<IEnumerable> GetAutoCompleteEntries()
        {
            return Task.FromResult<IEnumerable>(AutoCompleteEntries);
        }

        public async Task SelectAsync(FileExplorerEntry value)
        {
            await Selection.LookupAsync(value,
                RecrusiveSearch<DirectoryViewModel, FileExplorerEntry>.LoadSubentriesIfNotLoaded,
                SetSelected<DirectoryViewModel, FileExplorerEntry>.WhenSelected,
                SetExpanded<DirectoryViewModel, FileExplorerEntry>.WhenChildSelected);
        }

        public void InitializeRootElements(RootElementsDto dto)
        {
            var rootElements = dto.RootDirectories.ToList();
            rootElements.Add(dto.ComputerDirectory);

            RootViewModels = rootElements.Select(x =>
                    new DirectoryViewModel(this, null, x, _fileExplorerViewModel.FileSystem, _fileExplorerViewModel))
                .ToList();
            Entries.UpdateEntries(RootViewModels);

            InitializeRoots(dto.ComputerDirectory.Yield()).Forget(); //will execute synchronously

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (Action) (() =>
            {
                //expand and select Computer
                var entry = Entries.AllNonBindable.Last();
                entry.Entries.IsExpanded = true;

                Selection.AsRoot().SelectAsync(dto.ComputerDirectory).Forget();
                entry.BringIntoView();
            }));
        }

        public async Task InitializeRoots(IEnumerable<DirectoryEntry> directories)
        {
            var list = Entries.All.ToList(); //copy

            //load the entries of the root view models
            var tasks = directories.Select(entry => Entries.All.First(x => x.Source == entry).Entries.LoadAsync())
                .ToList();
            await Task.WhenAll(tasks);

            foreach (var task in tasks)
                list.AddRange(task.Result);

            AutoCompleteEntries = new ObservableCollection<DirectoryViewModel>(list);
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            var rootSelector = Selection.AsRoot();
            var currentItem = rootSelector.SelectedViewModel;
            //currentItem.BringIntoView();
            if (currentItem.Parent != null)
                currentItem.Parent.Entries.IsExpanded = true;

            SelectedViewModel = currentItem;

            _fileExplorerViewModel.OpenPath(currentItem.Source.Path).Forget();
        }

        private async void FileExplorerViewModelOnPathChanged(object sender, PathContent e)
        {
            var rootSelection = Selection.AsRoot();

            DirectoryViewModel directoryViewModel = null;
            if (rootSelection.IsChildSelected)
            {
                var relation = _fileExplorerPathComparer.CompareHierarchy(rootSelection.SelectedValue.Path, e.Path);

                switch (relation)
                {
                    case HierarchicalResult.Current:
                        return;
                    case HierarchicalResult.Parent:
                        directoryViewModel = UpwardSelect(e.Path, rootSelection.SelectedViewModel);
                        break;
                    case HierarchicalResult.Child:
                        directoryViewModel =
                            await DownwardSelect(e.PathDirectories, 0, rootSelection.SelectedViewModel);
                        break;
                    case HierarchicalResult.Unrelated:
                        directoryViewModel = null;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (directoryViewModel == null)
            {
                //check if there is any root view model which the path starts with (or is the path)

                var (rootTreeNodeViewModel, relation) = FindRelatedViewModel(Entries.All, e.Directory);

                if (rootTreeNodeViewModel == null)
                    (rootTreeNodeViewModel, relation) = FindRelatedViewModel(
                        Entries.All.First(x => ((DirectoryEntry) x.Source).IsComputerDirectory()).Entries.All,
                        e.Directory);

                if (rootTreeNodeViewModel != null)
                {
                    switch (relation)
                    {
                        case HierarchicalResult.Current:
                            directoryViewModel = rootTreeNodeViewModel;
                            break;
                        case HierarchicalResult.Child:
                            var position = e.PathDirectories.FindIndex(x =>
                                ComparePaths(x.Path, rootTreeNodeViewModel.Source.Path));

                            rootTreeNodeViewModel.Entries.IsExpanded = true;

                            directoryViewModel =
                                await DownwardSelect(e.PathDirectories, position + 1, rootTreeNodeViewModel);
                            break;
                    }
                }
            }

            if (rootSelection.IsChildSelected)
                rootSelection.SelectedViewModel.Selection.IsSelected = false;

            if (directoryViewModel == null)
            {
                //WTF
                return;
            }

            if (!directoryViewModel.Entries.IsLoaded)
                await directoryViewModel.Entries.LoadAsync();

            directoryViewModel.Selection.IsSelected = true;
            directoryViewModel.BringIntoView();
        }

        private (DirectoryViewModel, HierarchicalResult) FindRelatedViewModel(IEnumerable<DirectoryViewModel> entries,
            DirectoryEntry directoryEntry)
        {
            var items = entries
                .Select(directoryVm => (directoryVm,
                    _fileExplorerPathComparer.CompareHierarchy(directoryVm.Source, directoryEntry)))
                .Where(x => (x.Item2 & HierarchicalResult.Related) != 0 && x.Item2 != HierarchicalResult.Parent)
                .OrderByDescending(x => x.directoryVm.Source.Path.Length);
            return items.FirstOrDefault();
        }

        private DirectoryViewModel UpwardSelect(string path, DirectoryViewModel directoryViewModel)
        {
            while (true)
            {
                directoryViewModel = directoryViewModel.Parent;

                if (directoryViewModel == null)
                    return null;

                if (ComparePaths(directoryViewModel.Source.Path, path))
                    return directoryViewModel;
            }
        }

        private async Task<DirectoryViewModel> DownwardSelect(IReadOnlyList<DirectoryEntry> entries, int index,
            DirectoryViewModel directoryViewModel)
        {
            var currentEntry = entries[index];
            if (!directoryViewModel.Entries.IsLoaded)
                await directoryViewModel.Entries.LoadAsync();

            foreach (var viewModel in directoryViewModel.Entries.All)
            {
                if (ComparePaths(currentEntry.Path, viewModel.Source.Path))
                {
                    if (index == entries.Count - 1) //we are at the end
                        return viewModel;

                    var result = await DownwardSelect(entries, index + 1, viewModel);
                    viewModel.Entries.IsExpanded = true;
                    return result;
                }
            }

            return null;
        }

        private bool ComparePaths(string path1, string path2) =>
            _fileExplorerViewModel.FileSystem.ComparePaths(path1, path2);
    }
}