using FileExplorer.Administration.Models;
using FileExplorer.Administration.ViewModels.Explorer;
using FileExplorer.Administration.ViewModels.Explorer.Helpers;
using Orcus.Utilities;
using Prism.Commands;
using Prism.Mvvm;

namespace FileExplorer.Administration.ViewModels
{
    public class NavigationBarViewModel : BindableBase
    {
        private readonly FileExplorerViewModel _fileExplorerViewModel;

        private DelegateCommand _goBackCommand;
        private DelegateCommand _goForwardCommand;

        public NavigationBarViewModel(FileExplorerViewModel fileExplorerViewModel)
        {
            _fileExplorerViewModel = fileExplorerViewModel;
            _fileExplorerViewModel.PathChanged += FileExplorerViewModelOnPathChanged;
            PathHistoryManager = new PathHistoryManager(fileExplorerViewModel.FileSystem);

            DirectoryTreeViewModel = fileExplorerViewModel.DirectoryTreeViewModel;
        }

        public PathHistoryManager PathHistoryManager { get; }
        public DirectoryTreeViewModel DirectoryTreeViewModel { get; }

        public DelegateCommand GoBackCommand
        {
            get
            {
                return _goBackCommand ?? (_goBackCommand =
                           new DelegateCommand(() =>
                           {
                               _fileExplorerViewModel.OpenPath(PathHistoryManager.GoBack()).Forget();
                           }).ObservesCanExecute(() => PathHistoryManager.CanGoBack));
            }
        }

        public DelegateCommand GoForwardCommand
        {
            get
            {
                return _goForwardCommand ?? (_goForwardCommand =
                           new DelegateCommand(() =>
                           {
                               _fileExplorerViewModel.OpenPath(PathHistoryManager.GoForward()).Forget();
                           }).ObservesCanExecute(() => PathHistoryManager.CanGoForward));
            }
        }

        private void FileExplorerViewModelOnPathChanged(object sender, PathContent e)
        {
            PathHistoryManager.Navigate(e.Path);
        }
    }
}