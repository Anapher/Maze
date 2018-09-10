using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using AutoMapper;
using Orcus.Administration.ControllerExtensions;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Extensions;
using Orcus.Administration.Library.StatusBar;
using Orcus.Administration.Library.ViewModels;
using Prism.Commands;
using Prism.Regions;
using TaskManager.Administration.Rest;
using TaskManager.Shared.Channels;
using TaskManager.Shared.Dtos;

namespace TaskManager.Administration.ViewModel
{
    public class TaskManagerViewModel : ViewModelBase
    {
        private readonly IShellStatusBar _statusBar;
        private readonly IPackageRestClient _restClient;
        private CallTransmissionChannel<IProcessesProvider> _processProviderChannel;
        private Dictionary<int, ProcessViewModel> _processes;
        private string _searchText;
        private ListCollectionView _processView;
        private ObservableCollection<ProcessViewModel> _treeProcessViewModels;

        public TaskManagerViewModel(IShellStatusBar statusBar, ITargetedRestClient restClient)
        {
            _statusBar = statusBar;
            _restClient = restClient.CreatePackageSpecific("TaskManager");
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                    UpdateView();
            }
        }

        public ListCollectionView ProcessView
        {
            get => _processView;
            set => SetProperty(ref _processView, value);
        }

        private DelegateCommand<ProcessViewModel> _killProcessCommand;

        public DelegateCommand<ProcessViewModel> KillProcessCommand
        {
            get
            {
                return _killProcessCommand ?? (_killProcessCommand = new DelegateCommand<ProcessViewModel>(parameter =>
                {
                    
                }));
            }
        }

        private DelegateCommand<ProcessViewModel> _killProcessTreeCommand;

        public DelegateCommand<ProcessViewModel> KillProcessTreeCommand
        {
            get
            {
                return _killProcessTreeCommand ?? (_killProcessTreeCommand = new DelegateCommand<ProcessViewModel>(parameter =>
                {
                    
                }));
            }
        }

        private async Task LoadProcesses()
        {
            var processes = await _processProviderChannel.Interface.GetProcesses();
            foreach (var process in processes)
            {
                switch (process.Action)
                {
                    case EntryAction.Add:
                        var dto = process.Value;
                        if (_processes.TryGetValue(dto.Id, out var processViewModel))
                        {
                            Mapper.Map(dto, processViewModel);
                        }
                        else
                        {
                            processViewModel = Mapper.Map<ProcessViewModel>(dto);
                            processViewModel.CollectionView.Filter = Filter;
                            processViewModel.UpdateIcon(dto.IconData);

                            _processes.Add(processViewModel.Id, processViewModel);
                        }
                        break;
                    case EntryAction.Remove:
                        if (_processes.TryGetValue(process.Value.Id, out var viewModel))
                        {
                            _processes.Remove(process.Value.Id);

                            foreach (var viewModelChild in viewModel.Childs)
                            {
                                viewModelChild.ParentViewModel = null;
                                _treeProcessViewModels.Add(viewModelChild);
                            }

                            if (viewModel.ParentViewModel == null)
                                _treeProcessViewModels.Remove(viewModel);
                            else viewModel.ParentViewModel.Childs.Remove(viewModel);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            foreach (var keyValuePair in _processes)
            {
                var viewModel = keyValuePair.Value;
                if (viewModel.ParentProcess != 0 && _processes.TryGetValue(viewModel.ParentProcess, out var parentViewModel))
                {
                    if (parentViewModel != viewModel.ParentViewModel)
                    {
                        if (viewModel.ParentViewModel == null)
                            _treeProcessViewModels.Remove(viewModel);
                        else viewModel.ParentViewModel.Childs.Remove(viewModel);

                        viewModel.ParentViewModel = parentViewModel;
                        parentViewModel.Childs.Add(viewModel);
                    }
                }
            }
        }

        public void UpdateView()
        {
            ProcessView.Refresh();

            foreach (var processViewModel in _processes.Where(x => x.Value.Childs.Any()))
            {
                processViewModel.Value.UpdateView();
            }
        }

        private bool Filter(object obj)
        {
            var processViewModel = (ProcessViewModel) obj;

            if (string.IsNullOrWhiteSpace(SearchText))
                return true;

            return MatchesSearchPattern(processViewModel);
        }

        private bool MatchesSearchPattern(ProcessViewModel processViewModel)
        {
            if (processViewModel.Name.IndexOf(_searchText, StringComparison.OrdinalIgnoreCase) > -1)
                return true;

            return processViewModel.Childs.Any(MatchesSearchPattern);
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            _processProviderChannel = await TaskManagerResource.GetProcessProvider(_restClient);
            _processes = new Dictionary<int, ProcessViewModel>();
            _treeProcessViewModels = new ObservableCollection<ProcessViewModel>();

            ProcessView = new ListCollectionView(_treeProcessViewModels) {Filter = Filter};

            await LoadProcesses();
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
            _processProviderChannel?.Dispose();
            _processes = null;
        }
    }
}