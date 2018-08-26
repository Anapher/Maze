using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using FileExplorer.Administration.Controls.Models;
using FileExplorer.Administration.Helpers;
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

            RootViewModels = rootElements
                .Select(x => new DirectoryNodeViewModel(this, null, x, _fileExplorerViewModel.FileSystem, _fileExplorerViewModel)).ToList();
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

            _fileExplorerViewModel.OpenPath(currentItem.Source.Path).Forget();
        }
    }
}