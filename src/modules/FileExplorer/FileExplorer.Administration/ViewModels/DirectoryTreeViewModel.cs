using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using FileExplorer.Administration.Controls.Models;
using FileExplorer.Administration.Helpers;
using FileExplorer.Administration.Models;
using FileExplorer.Administration.ViewModels.Explorer;
using FileExplorer.Administration.ViewModels.Explorer.Helpers;
using FileExplorer.Shared.Dtos;
using Orcus.Utilities;
using Prism.Mvvm;

namespace FileExplorer.Administration.ViewModels
{
    public class DirectoryTreeViewModel : BindableBase, ISupportTreeSelector<DirectoryNodeViewModel, FileExplorerEntry>
    {
        private readonly FileExplorerViewModel _fileExplorerViewModel;
        private ObservableCollection<DirectoryNodeViewModel> _autoCompleteEntries;
        private List<DirectoryNodeViewModel> _rootViewModels;
        private DirectoryNodeViewModel _selectedViewModel;

        public DirectoryTreeViewModel(FileExplorerViewModel fileExplorerViewModel)
        {
            _fileExplorerViewModel = fileExplorerViewModel;
            NavigationBarViewModel = fileExplorerViewModel.NavigationBarViewModel;

            Entries = new EntriesHelper<DirectoryNodeViewModel>();
            Selection = new TreeRootSelector<DirectoryNodeViewModel, FileExplorerEntry>(Entries)
            {
                Comparers = new[] {new FileExplorerPathComparer(fileExplorerViewModel.FileSystem)}
            };
            Selection.AsRoot().SelectionChanged += OnSelectionChanged;
            _fileExplorerViewModel.PathChanged += FileExplorerViewModelOnPathChanged;
        }

        public NavigationBarViewModel NavigationBarViewModel { get; }

        public List<DirectoryNodeViewModel> RootViewModels
        {
            get => _rootViewModels;
            private set => SetProperty(ref _rootViewModels, value);
        }

        public ObservableCollection<DirectoryNodeViewModel> AutoCompleteEntries
        {
            get => _autoCompleteEntries;
            private set => SetProperty(ref _autoCompleteEntries, value);
        }

        public DirectoryNodeViewModel SelectedViewModel
        {
            get => _selectedViewModel;
            private set => SetProperty(ref _selectedViewModel, value);
        }

        public IEntriesHelper<DirectoryNodeViewModel> Entries { get; set; }
        public ITreeSelector<DirectoryNodeViewModel, FileExplorerEntry> Selection { get; set; }

        public async Task SelectAsync(FileExplorerEntry value)
        {
            await Selection.LookupAsync(value,
                RecrusiveSearch<DirectoryNodeViewModel, FileExplorerEntry>.LoadSubentriesIfNotLoaded,
                SetSelected<DirectoryNodeViewModel, FileExplorerEntry>.WhenSelected,
                SetExpanded<DirectoryNodeViewModel, FileExplorerEntry>.WhenChildSelected);
        }

        public void InitializeRootElements(RootElementsDto dto)
        {
            var rootElements = dto.RootDirectories.ToList();
            rootElements.Add(dto.ComputerDirectory);

            RootViewModels = rootElements.Select(x =>
                    new DirectoryNodeViewModel(this, null, x, _fileExplorerViewModel.FileSystem,
                        _fileExplorerViewModel))
                .ToList();
            Entries.SetEntries(UpdateMode.Update, RootViewModels.ToArray());

            InitializeRoots(dto.ComputerDirectory.Yield()).Forget(); //will execute synchronously

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (Action) (() =>
            {
                //expand and select Computer
                var entry = Entries.AllNonBindable.Last();
                entry.Entries.IsExpanded = true;

                Selection.AsRoot().SelectAsync(dto.ComputerDirectory).Forget();

                entry.IsBringIntoView = true;
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

            AutoCompleteEntries = new ObservableCollection<DirectoryNodeViewModel>(list);
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            var rootSelector = Selection.AsRoot();
            var currentItem = rootSelector.SelectedViewModel;
            currentItem.IsBringIntoView = true;
            if (currentItem.Parent != null)
                currentItem.Parent.Entries.IsExpanded = true;

            SelectedViewModel = currentItem;

            _fileExplorerViewModel.OpenPath(currentItem.Source.Path).Forget();
        }

        private async void FileExplorerViewModelOnPathChanged(object sender, PathContent e)
        {
            bool ComparePaths(string path1, string path2) =>
                _fileExplorerViewModel.FileSystem.ComparePaths(path1, path2);

            var rootSelection = Selection.AsRoot();

            //the directory is already selected in the tree
            if (rootSelection.IsChildSelected && ComparePaths(e.Path, rootSelection.SelectedValue.Path))
                return;

            //when a subdirectory is selected
            if (rootSelection.SelectedViewModel != null)
            {
                var selectedViewModel = rootSelection.SelectedViewModel;
                if (selectedViewModel.Entries.IsLoaded)
                {
                    var subEntry =
                        selectedViewModel.Entries.All.FirstOrDefault(x => ComparePaths(x.Source.Path, e.Path));
                    if (subEntry != null)
                    {
                        selectedViewModel.Selection.IsSelected = false;
                        subEntry.Entries.IsExpanded = true;
                        subEntry.Selection.IsSelected = true;
                        subEntry.IsBringIntoView = true;
                        return;
                    }
                }
            }

            //check if there is any root view model which the path starts with
            var rootTreeNodeViewModel = Entries.All.FirstOrDefault(x =>
                e.Path.StartsWith(x.Source.Path, _fileExplorerViewModel.FileSystem.PathStringComparison));

            if (rootTreeNodeViewModel != null && (rootTreeNodeViewModel.Selection.IsSelected || rootTreeNodeViewModel.Selection.IsChildSelected))
            {
                //if the path is equal to the root view model, we just select it
                if (ComparePaths(rootTreeNodeViewModel.Source.Path, e.Path))
                {
                    if (!rootTreeNodeViewModel.Entries.IsLoaded)
                        await rootTreeNodeViewModel.Entries.LoadAsync();

                    rootTreeNodeViewModel.Entries.IsExpanded = true;
                    rootTreeNodeViewModel.Selection.IsSelected = true;
                    rootTreeNodeViewModel.IsBringIntoView = true;
                }
                else
                {
                    //it may be possible that the rootTreeNodeViewModel is a path with multiple directories like C:\OneDrive. We have to find the current position
                    var currentPathPart =
                        e.PathDirectories.First(x => ComparePaths(x.Path, rootTreeNodeViewModel.Source.Path));

                    RecursiveSelect(e.PathDirectories, e.PathDirectories.IndexOf(currentPathPart),
                        rootTreeNodeViewModel).Forget();
                }
                return;
            }

            var pathRoot = e.PathDirectories.First();

            DirectoryNodeViewModel rootEntry = null;
            foreach (var directoryNodeViewModel in Entries.All)
            {
                if (directoryNodeViewModel.Source.Equals(pathRoot))
                {
                    if (!directoryNodeViewModel.Entries.IsLoaded)
                        await directoryNodeViewModel.Entries.LoadAsync();

                    directoryNodeViewModel.Entries.IsExpanded = true;
                    directoryNodeViewModel.Selection.IsSelected = true;
                    directoryNodeViewModel.IsBringIntoView = true;
                    return;
                }

                if (directoryNodeViewModel.Entries.IsLoaded)
                {
                    rootEntry = directoryNodeViewModel.Entries.All.FirstOrDefault(x =>
                                    e.Path.StartsWith(x.Source.Path,
                                        _fileExplorerViewModel.FileSystem.PathStringComparison)) ??
                                directoryNodeViewModel.Entries.All.FirstOrDefault(x => x.Source == pathRoot);
                    if (rootEntry != null)
                    {
                        var pathDirectory = e.PathDirectories.First(x => ComparePaths(x.Path, rootEntry.Source.Path));

                        await RecursiveSelect(e.PathDirectories, e.PathDirectories.IndexOf(pathDirectory) + 1,
                            rootEntry);
                        rootEntry.Entries.IsExpanded = true;
                    }
                }
            }
        }

        private async Task<List<DirectoryNodeViewModel>> RecursiveSelect(IReadOnlyList<DirectoryEntry> entries,
            int index, DirectoryNodeViewModel directoryNodeViewModel)
        {
            var currentEntry = entries[index];
            if (!directoryNodeViewModel.Entries.IsLoaded)
                await directoryNodeViewModel.Entries.LoadAsync();

            foreach (var viewModel in directoryNodeViewModel.Entries.All)
            {
                if (viewModel.Source.Equals(currentEntry))
                {
                    if (index == entries.Count - 1)
                    {
                        viewModel.Selection.IsSelected = true;
                        viewModel.IsBringIntoView = true;
                        if (!viewModel.Entries.IsLoaded)
                            await viewModel.Entries.LoadAsync();
                        return viewModel.Entries.All.ToList();
                    }

                    var result = await RecursiveSelect(entries, index + 1, viewModel);
                    viewModel.Entries.IsExpanded = true;
                    return result;
                }
            }

            return null;
        }
    }
}