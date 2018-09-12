using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using Orcus.Administration.ControllerExtensions;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Extensions;
using Orcus.Administration.Library.StatusBar;
using Orcus.Administration.Library.ViewModels;
using Orcus.Utilities;
using Prism.Commands;
using Prism.Regions;
using TaskManager.Administration.Rest;
using TaskManager.Shared.Channels;
using Unclassified.TxLib;

namespace TaskManager.Administration.ViewModels
{
    public class TaskManagerViewModel : ViewModelBase
    {
        private readonly IPackageRestClient _restClient;
        private readonly IShellStatusBar _statusBar;

        private DelegateCommand<ProcessViewModel> _bringToFrontCommand;
        private DelegateCommand<ProcessViewModel> _closeWindowCommand;
        private DelegateCommand<ProcessViewModel> _killProcessCommand;
        private DelegateCommand<ProcessViewModel> _killProcessTreeCommand;
        private DelegateCommand<ProcessViewModel> _maximizeWindowCommand;
        private DelegateCommand<ProcessViewModel> _minimizeWindowCommand;
        private Dictionary<int, ProcessViewModel> _processes;
        private CallTransmissionChannel<IProcessesProvider> _processProviderChannel;
        private ListCollectionView _processView;
        private DelegateCommand _refreshCommand;
        private DelegateCommand<ProcessViewModel> _restoreWindowCommand;
        private DelegateCommand<ProcessViewModel> _resumeCommand;
        private string _searchText;
        private DelegateCommand<ProcessViewModel> _showPropertiesCommand;
        private DelegateCommand<ProcessViewModel> _suspendCommand;
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

        public DelegateCommand RefreshCommand
        {
            get { return _refreshCommand ?? (_refreshCommand = new DelegateCommand(() => { LoadProcesses().Forget(); })); }
        }

        public DelegateCommand<ProcessViewModel> KillProcessCommand
        {
            get
            {
                return _killProcessCommand ?? (_killProcessCommand = new DelegateCommand<ProcessViewModel>(parameter =>
                           {
                               ProcessesResource.Kill(parameter.Id, _restClient).DisplayOnStatusBarCatchErrors(_statusBar,
                                   Tx.T("TaskManager:StatusBar.KillProcess", "name", parameter.Name), StatusBarAnimation.None, mustShow: true)
                               .Forget();
                           }));
            }
        }

        public DelegateCommand<ProcessViewModel> KillProcessTreeCommand
        {
            get
            {
                return _killProcessTreeCommand ?? (_killProcessTreeCommand = new DelegateCommand<ProcessViewModel>(parameter =>
                {
                    ProcessesResource.KillTree(parameter.Id, _restClient).DisplayOnStatusBarCatchErrors(_statusBar,
                        Tx.T("TaskManager:StatusBar.KillProcessTree", "name", parameter.Name), StatusBarAnimation.None, mustShow: true).Forget();
                }));
            }
        }

        public DelegateCommand<ProcessViewModel> BringToFrontCommand
        {
            get
            {
                return _bringToFrontCommand ?? (_bringToFrontCommand = new DelegateCommand<ProcessViewModel>(parameter =>
                {
                    ProcessesWindowResource.BringToFront(parameter.Id, _restClient)
                        .DisplayOnStatusBarCatchErrors(_statusBar, Tx.T("TaskManager:StatusBar.BringToFront", "name", parameter.Name), StatusBarAnimation.None, mustShow: true)
                        .Forget();
                }, model => model.MainWindowHandle != null));
            }
        }

        public DelegateCommand<ProcessViewModel> RestoreWindowCommand
        {
            get
            {
                return _restoreWindowCommand ?? (_restoreWindowCommand = new DelegateCommand<ProcessViewModel>(parameter =>
                {
                    ProcessesWindowResource.Restore(parameter.Id, _restClient).DisplayOnStatusBarCatchErrors(_statusBar,
                        Tx.T("TaskManager:StatusBar.RestoreWindow", "name", parameter.Name), StatusBarAnimation.None, mustShow: true).Forget();
                }, model => model.MainWindowHandle != null));
            }
        }

        public DelegateCommand<ProcessViewModel> MinimizeWindowCommand
        {
            get
            {
                return _minimizeWindowCommand ?? (_minimizeWindowCommand = new DelegateCommand<ProcessViewModel>(parameter =>
                {
                    ProcessesWindowResource.Minimize(parameter.Id, _restClient).DisplayOnStatusBarCatchErrors(_statusBar,
                        Tx.T("TaskManager:StatusBar.MinimizeWindow", "name", parameter.Name), StatusBarAnimation.None, mustShow: true).Forget();
                }, model => model.MainWindowHandle != null));
            }
        }

        public DelegateCommand<ProcessViewModel> MaximizeWindowCommand
        {
            get
            {
                return _maximizeWindowCommand ?? (_maximizeWindowCommand = new DelegateCommand<ProcessViewModel>(parameter =>
                {
                    ProcessesWindowResource.Maximize(parameter.Id, _restClient).DisplayOnStatusBarCatchErrors(_statusBar,
                        Tx.T("TaskManager:StatusBar.MaximizeWindow", "name", parameter.Name), StatusBarAnimation.None, mustShow: true).Forget();
                }, model => model.MainWindowHandle != null));
            }
        }

        public DelegateCommand<ProcessViewModel> CloseWindowCommand
        {
            get
            {
                return _closeWindowCommand ?? (_closeWindowCommand = new DelegateCommand<ProcessViewModel>(parameter =>
                {
                    ProcessesWindowResource.Close(parameter.Id, _restClient).DisplayOnStatusBarCatchErrors(_statusBar,
                        Tx.T("TaskManager:StatusBar.CloseWindow", "name", parameter.Name), StatusBarAnimation.None, mustShow: true).Forget();
                }, model => model.MainWindowHandle != null));
            }
        }

        public DelegateCommand<ProcessViewModel> SuspendCommand
        {
            get
            {
                return _suspendCommand ?? (_suspendCommand = new DelegateCommand<ProcessViewModel>(parameter =>
                {
                    ProcessesResource.Suspend(parameter.Id, _restClient).DisplayOnStatusBarCatchErrors(_statusBar,
                        Tx.T("TaskManager:StatusBar.Suspend", "name", parameter.Name), StatusBarAnimation.None, mustShow: true).Forget();
                }));
            }
        }

        public DelegateCommand<ProcessViewModel> ResumeCommand
        {
            get
            {
                return _resumeCommand ?? (_resumeCommand = new DelegateCommand<ProcessViewModel>(parameter =>
                {
                    ProcessesResource.Resume(parameter.Id, _restClient).DisplayOnStatusBarCatchErrors(_statusBar,
                        Tx.T("TaskManager:StatusBar.Resume", "name", parameter.Name), StatusBarAnimation.None, mustShow: true).Forget();
                }));
            }
        }

        private DelegateCommand<object[]> _setPriorityCommand;

        public DelegateCommand<object[]> SetPriorityCommand
        {
            get
            {
                return _setPriorityCommand ?? (_setPriorityCommand = new DelegateCommand<object[]>(async parameter =>
                {
                    var processViewModel = ((ProcessViewModel) parameter[0]);
                    var priority = (ProcessPriorityClass) parameter[1];

                    processViewModel.UpdatePriorityClass();

                    if (await ProcessesResource.SetPriority(processViewModel.Id, priority, _restClient).DisplayOnStatusBarCatchErrors(_statusBar,
                        Tx.T("TaskManager:StatusBar.SetPriority", "name", processViewModel.Name, "priority", priority.ToString()),
                        StatusBarAnimation.None, mustShow: true))
                    {
                        processViewModel.PriorityClass = priority;
                    }
                }));
            }
        }

        public DelegateCommand<ProcessViewModel> ShowPropertiesCommand
        {
            get { return _showPropertiesCommand ?? (_showPropertiesCommand = new DelegateCommand<ProcessViewModel>(parameter => { })); }
        }

        private async Task LoadProcesses()
        {
            var result = await _processProviderChannel.Interface.GetProcesses()
                .DisplayOnStatusBarCatchErrors(_statusBar, Tx.T("TaskManager:LoadProcesses"));
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

            foreach (var keyValuePair in _processes)
            {
                var viewModel = keyValuePair.Value;
                if (viewModel.ParentProcessId != 0 && _processes.TryGetValue(viewModel.ParentProcessId, out var parentViewModel))
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

        public void UpdateView()
        {
            ProcessView.Refresh();

            foreach (var processViewModel in _processes.Where(x => x.Value.Childs.Any())) processViewModel.Value.UpdateView();
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