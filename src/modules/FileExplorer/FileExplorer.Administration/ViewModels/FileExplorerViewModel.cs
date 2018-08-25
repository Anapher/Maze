using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Anapher.Wpf.Swan.ViewInterface;
using FileExplorer.Administration.Models;
using Microsoft.Extensions.Caching.Memory;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Exceptions;
using Orcus.Administration.Library.Extensions;
using Orcus.Administration.Library.StatusBar;
using Prism.Mvvm;
using Prism.Regions;
using Unclassified.TxLib;

namespace FileExplorer.Administration.ViewModels
{
    public class FileExplorerViewModel : BindableBase, INavigationAware
    {
        private string _currentPath;
        private CancellationTokenSource _openPathCancellationTokenSource;
        private bool _isLoaded;

        public FileExplorerViewModel(IShellStatusBar statusBar, IWindow window, IMemoryCache cache, ITargetedRestClient client)
        {
            StatusBar = statusBar;
            Window = window;
            RestClient = client.CreatePackageSpecific("FileExplorer");
            FileSystem = new RemoteFileSystem(cache, RestClient);

            EntriesViewModel = new EntriesViewModel(this);
            NavigationBarViewModel = new NavigationBarViewModel(this);
            DirectoryTreeViewModel = new DirectoryTreeViewModel(this);
        }

        public event EventHandler<PathContent> PathChanged;

        public IPackageRestClient RestClient { get; }
        public IShellStatusBar StatusBar { get; }
        public IWindow Window { get; }
        public IFileSystem FileSystem { get; }

        public EntriesViewModel EntriesViewModel { get; }
        public NavigationBarViewModel NavigationBarViewModel { get; }
        public DirectoryTreeViewModel DirectoryTreeViewModel { get; }

        public string CurrentPath
        {
            get => _currentPath;
            private set => SetProperty(ref _currentPath, value);
        }

        public async Task OpenPath(string path, bool invalidate = false)
        {
            if (!invalidate && FileSystem.ComparePaths(path, CurrentPath))
                return;

            _openPathCancellationTokenSource?.Cancel();
            _openPathCancellationTokenSource = new CancellationTokenSource();

            var token = _openPathCancellationTokenSource.Token;
            PathContent pathContent;

            try
            {
                pathContent = await FileSystem.FetchPath(path, invalidate, invalidate, token)
                    .DisplayOnStatusBar(StatusBar, "");
            }
            catch (TaskCanceledException)
            {
                return;
            }
            catch (RestException e)
            {
                StatusBar.ShowError(e.GetRestExceptionMessage());
                return;
            }
            catch (Exception e)
            {
                StatusBar.ShowError(e.Message);
                return;
            }

            if (token.IsCancellationRequested)
                return;

            CurrentPath = pathContent.Path; //correct casing etc.

            PathChanged?.Invoke(this, pathContent);
        }

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (_isLoaded)
                return;

            _isLoaded = true;

            var root = await FileSystem.GetRoot()
                .DisplayOnStatusBar(StatusBar, Tx.T("FileExplorer:StatusBar.LoadingRoot"));

            DirectoryTreeViewModel.InitializeRootElements(root);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
    }
}