using System;
using System.ComponentModel;
using Anapher.Wpf.Swan;
using FileExplorer.Administration.Models;
using FileExplorer.Administration.ViewModels.Explorer;
using FileExplorer.Administration.ViewModels.Explorer.Base;
using FileExplorer.Administration.ViewModels.Explorer.Helpers;
using Orcus.Administration.Library.Extensions;
using Orcus.Utilities;
using Prism.Commands;
using Prism.Mvvm;

namespace FileExplorer.Administration.ViewModels
{
    public class NavigationBarViewModel : BindableBase, IFileExplorerChildViewModel
    {
        private DelegateCommand _goBackCommand;
        private DelegateCommand _goForwardCommand;
        private DelegateCommand<string> _navigateToPathCommand;
        private AsyncRelayCommand _refreshEntriesCommand;
        private string _searchText;

        public void Initialize(FileExplorerViewModel fileExplorerViewModel)
        {
            FileExplorerViewModel = fileExplorerViewModel;
            FileExplorerViewModel.PathChanged += FileExplorerViewModelOnPathChanged;
            PathHistoryManager = new PathHistoryManager(fileExplorerViewModel.FileSystem);
            PathHistoryManager.PropertyChanged += PathHistoryManagerOnPropertyChanged;

            fileExplorerViewModel.EntriesViewModel.Filters.Add(FilterItems);
            fileExplorerViewModel.EntriesViewModel.LiveFilteringProperties.Add(nameof(EntryViewModel.Label));
        }

        public PathHistoryManager PathHistoryManager { get; private set; }
        public DirectoryTreeViewModel DirectoryTreeViewModel => FileExplorerViewModel.DirectoryTreeViewModel;
        public FileExplorerViewModel FileExplorerViewModel { get; private set; }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                    FileExplorerViewModel.EntriesViewModel.View.SafeRefresh();
            }
        }

        public DelegateCommand GoBackCommand
        {
            get
            {
                return _goBackCommand ?? (_goBackCommand = new DelegateCommand(
                           () => { FileExplorerViewModel.OpenPath(PathHistoryManager.GoBack()).Forget(); },
                           () => PathHistoryManager.CanGoBack));
            }
        }

        public DelegateCommand GoForwardCommand
        {
            get
            {
                return _goForwardCommand ?? (_goForwardCommand = new DelegateCommand(
                           () => { FileExplorerViewModel.OpenPath(PathHistoryManager.GoForward()).Forget(); },
                           () => PathHistoryManager.CanGoForward));
            }
        }

        public DelegateCommand<string> NavigateToPathCommand
        {
            get
            {
                return _navigateToPathCommand ?? (_navigateToPathCommand = new DelegateCommand<string>(parameter =>
                {
                    FileExplorerViewModel.OpenPath(parameter).Forget();
                }));
            }
        }

        public AsyncRelayCommand RefreshEntriesCommand
        {
            get
            {
                return _refreshEntriesCommand ?? (_refreshEntriesCommand = new AsyncRelayCommand(parameter =>
                           FileExplorerViewModel.OpenPath(FileExplorerViewModel.CurrentPath, true)));
            }
        }

        private void FileExplorerViewModelOnPathChanged(object sender, PathContent e)
        {
            PathHistoryManager.Navigate(e.Path);
        }

        private void PathHistoryManagerOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PathHistoryManager.CanGoBack))
                GoBackCommand.RaiseCanExecuteChanged();
            else if (e.PropertyName == nameof(PathHistoryManager.CanGoForward))
                GoForwardCommand.RaiseCanExecuteChanged();
        }

        private bool FilterItems(object obj)
        {
            if (string.IsNullOrEmpty(_searchText))
                return true;

            var entryVm = (EntryViewModel) obj;
            return entryVm.Label.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) > -1;
        }
    }
}