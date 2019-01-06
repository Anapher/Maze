using System;
using Anapher.Wpf.Toolkit;
using Anapher.Wpf.Toolkit.Windows;
using FileExplorer.Administration.Rest;
using FileExplorer.Administration.ViewModels.Explorer;
using FileExplorer.Shared.Dtos;
using Maze.Administration.Library.Clients;
using Maze.Administration.Library.Extensions;
using Maze.Administration.Library.ViewModels;
using Prism.Regions;

namespace FileExplorer.Administration.ViewModels
{
    public class ExecuteFileViewModel : ViewModelBase
    {
        private readonly IWindowService _windowService;
        private FileViewModel _fileViewModel;
        private ITargetedRestClient _restClient;
        private string[] _availableVerbs;
        private bool? _dialogResult;
        private AsyncDelegateCommand _executeCommand;
        private string _filename;

        public ExecuteFileViewModel(IWindowService windowService)
        {
            _windowService = windowService;
        }

        public void Initialize(FileViewModel fileViewModel, ITargetedRestClient restClient)
        {
            _fileViewModel = fileViewModel;
            _restClient = restClient;
            ExecuteDto = new ExecuteFileDto { FileName = fileViewModel.Source.Path };

            if (!ExecuteDto.FileName.EndsWith(".exe"))
                ExecuteDto.UseShellExecute = true;
        }

        public ExecuteFileDto ExecuteDto { get; private set; }

        public bool? DialogResult
        {
            get => _dialogResult;
            set => SetProperty(ref _dialogResult, value);
        }

        public string[] AvailableVerbs
        {
            get => _availableVerbs;
            set => SetProperty(ref _availableVerbs, value);
        }

        public string Filename
        {
            get => _filename;
            set
            {
                if (SetProperty(ref _filename, value))
                {
                    ExecuteDto.FileName = value;
                    FileSystemResource.GetFileVerbs(_fileViewModel.Source.Path, _restClient).ContinueWith(task =>
                    {
                        if (!task.IsFaulted)
                            AvailableVerbs = task.Result;
                    });
                }
            }
        }

        public AsyncDelegateCommand ExecuteCommand
        {
            get
            {
                return _executeCommand ?? (_executeCommand = new AsyncDelegateCommand(async () =>
                {
                    try
                    {
                        await FileSystemResource.ExecuteFile(ExecuteDto, false, _restClient);
                        DialogResult = true;
                    }
                    catch (Exception e)
                    {
                        e.ShowMessage(_windowService);
                    }
                }));
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            Filename = _fileViewModel.Source.Path;
        }
    }
}