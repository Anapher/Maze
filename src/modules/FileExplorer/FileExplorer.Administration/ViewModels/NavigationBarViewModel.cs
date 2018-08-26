using System.ComponentModel;
using FileExplorer.Administration.Models;
using FileExplorer.Administration.ViewModels.Explorer.Helpers;
using Orcus.Utilities;
using Prism.Commands;
using Prism.Mvvm;

namespace FileExplorer.Administration.ViewModels
{
    public class NavigationBarViewModel : BindableBase
    {
        private DelegateCommand _goBackCommand;
        private DelegateCommand _goForwardCommand;
        private DelegateCommand<string> _navigateToPathCommand;

        public NavigationBarViewModel(FileExplorerViewModel fileExplorerViewModel)
        {
            FileExplorerViewModel = fileExplorerViewModel;
            FileExplorerViewModel.PathChanged += FileExplorerViewModelOnPathChanged;
            PathHistoryManager = new PathHistoryManager(fileExplorerViewModel.FileSystem);
            PathHistoryManager.PropertyChanged += PathHistoryManagerOnPropertyChanged;
        }

        public PathHistoryManager PathHistoryManager { get; }
        public DirectoryTreeViewModel DirectoryTreeViewModel => FileExplorerViewModel.DirectoryTreeViewModel;
        public FileExplorerViewModel FileExplorerViewModel { get; }

        public DelegateCommand GoBackCommand
        {
            get
            {
                return _goBackCommand ?? (_goBackCommand =
                           new DelegateCommand(() =>
                           {
                               FileExplorerViewModel.OpenPath(PathHistoryManager.GoBack()).Forget();
                           }, () => PathHistoryManager.CanGoBack));
            }
        }

        public DelegateCommand GoForwardCommand
        {
            get
            {
                return _goForwardCommand ?? (_goForwardCommand =
                           new DelegateCommand(() =>
                           {
                               FileExplorerViewModel.OpenPath(PathHistoryManager.GoForward()).Forget();
                           }, () => PathHistoryManager.CanGoForward));
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
    }
}