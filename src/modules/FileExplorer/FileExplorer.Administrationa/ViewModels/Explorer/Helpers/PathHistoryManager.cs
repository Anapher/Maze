using System.Collections.Generic;
using FileExplorer.Administration.Models;
using Prism.Mvvm;

namespace FileExplorer.Administration.ViewModels.Explorer.Helpers
{
    public class PathHistoryManager : BindableBase
    {
        private readonly Stack<string> _backwardsStack;
        private readonly IFileSystem _fileSystem;
        private readonly Stack<string> _forwardStack;

        private string _currentPath;

        public PathHistoryManager(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _backwardsStack = new Stack<string>();
            _forwardStack = new Stack<string>();
        }

        public string CurrentPath
        {
            get => _currentPath;
            private set => SetProperty(ref _currentPath, value);
        }

        public bool CanGoBack => _backwardsStack.Count > 0;
        public bool CanGoForward => _forwardStack.Count > 0;

        public void Navigate(string path)
        {
            if (_fileSystem.ComparePaths(path, CurrentPath))
                return;

            _forwardStack.Clear();
            if (CurrentPath != null)
                _backwardsStack.Push(CurrentPath);

            CurrentPath = path;
            UpdateNavigation();
        }

        public string GoBack()
        {
            var path = _backwardsStack.Pop();
            _forwardStack.Push(CurrentPath);
            CurrentPath = path;

            UpdateNavigation();

            return path;
        }

        public string GoForward()
        {
            var path = _forwardStack.Pop();
            _backwardsStack.Push(CurrentPath);
            CurrentPath = path;

            UpdateNavigation();

            return path;
        }

        private void UpdateNavigation()
        {
            RaisePropertyChanged(nameof(CanGoBack));
            RaisePropertyChanged(nameof(CanGoForward));
        }
    }
}