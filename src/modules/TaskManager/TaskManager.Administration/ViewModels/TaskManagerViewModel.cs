using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
using Unclassified.TxLib;

namespace TaskManager.Administration.ViewModels
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
            var result = await _processProviderChannel.Interface.GetProcesses().DisplayOnStatusBarCatchErrors(_statusBar, Tx.T("TaskManager:LoadProcesses"));
            if (result.Failed)
                return;

            var processes = result.Result;
            var processesToDelete = new HashSet<int>(_processes.Keys);
            
            foreach (var process in processes)
            {
                processesToDelete.Remove(process.ProcessId);

                if (_processes.TryGetValue(process.ProcessId, out var processViewModel))
                {
                    processViewModel.Apply(process);
                }
                else
                {
                    processViewModel = new ProcessViewModel();
                    processViewModel.CollectionView.Filter = Filter;
                    processViewModel.CollectionView.SortDescriptions.Add(new SortDescription(nameof(ProcessViewModel.Name),
                        ListSortDirection.Ascending));
                    processViewModel.Apply(process);

                    _processes.Add(processViewModel.Id, processViewModel);
                    _treeProcessViewModels.Add(processViewModel);
                }
            }

            foreach (var processId in processesToDelete)
            {
                if (_processes.TryGetValue(processId, out var viewModel))
                {
                    _processes.Remove(processId);

                    foreach (var viewModelChild in viewModel.Childs)
                    {
                        viewModelChild.ParentViewModel = null;
                        _treeProcessViewModels.Add(viewModelChild);
                    }

                    if (viewModel.ParentViewModel == null)
                        _treeProcessViewModels.Remove(viewModel);
                    else viewModel.ParentViewModel.Childs.Remove(viewModel);
                }
            }
            
            foreach (var keyValuePair in _processes)
            {
                var viewModel = keyValuePair.Value;
                if (viewModel.ParentProcessId != 0 && _processes.TryGetValue(viewModel.ParentProcessId, out var parentViewModel))
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
            ProcessView.SortDescriptions.Add(new SortDescription(nameof(ProcessViewModel.CreationDate), ListSortDirection.Ascending));

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