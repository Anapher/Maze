using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Anapher.Wpf.Swan.Extensions;
using Autofac;
using MahApps.Metro.IconPacks;
using Microsoft.AspNetCore.SignalR.Client;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Extensions;
using Orcus.Administration.Library.Services;
using Orcus.Administration.Library.ViewModels;
using Orcus.Administration.Library.Views;
using Orcus.Utilities;
using Prism.Commands;
using Tasks.Infrastructure.Administration.Rest.V1;
using Tasks.Infrastructure.Administration.ViewModels.Overview;
using Tasks.Infrastructure.Core.Dtos;
using Unclassified.TxLib;

namespace Tasks.Infrastructure.Administration.ViewModels
{
    public class TasksViewModel : OverviewTabBase
    {
        private readonly IRestClient _restClient;
        private readonly IAppDispatcher _dispatcher;
        private readonly IComponentContext _services;
        private readonly IWindowService _windowService;
        private DelegateCommand _createTaskCommand;
        private bool _isTasksLoaded;
        private ObservableCollection<TaskViewModel> _tasks;
        private ICollectionView _tasksView;
        private DelegateCommand<TaskViewModel> _openTaskSessionsCommand;
        private DelegateCommand<TaskViewModel> _removeCommand;
        private DelegateCommand<TaskViewModel> _triggerCommand;

        public TasksViewModel(IWindowService windowService, IOrcusRestClient restClient, IAppDispatcher dispatcher, IComponentContext services) :
            base(Tx.T("TasksView:Tasks"), PackIconFontAwesomeKind.CalendarCheckRegular)
        {
            _windowService = windowService;
            _restClient = restClient;
            _dispatcher = dispatcher;
            _services = services;

            restClient.HubConnection.On<TaskExecutionDto>("TaskExecutionCreated", OnTaskExecutionCreated);
            restClient.HubConnection.On<TaskSessionDto>("TaskSessionCreated", OnTaskSessionCreated);
        }

        public ICollectionView TasksView
        {
            get => _tasksView;
            set => SetProperty(ref _tasksView, value);
        }

        public DelegateCommand CreateTaskCommand
        {
            get
            {
                return _createTaskCommand ?? (_createTaskCommand = new DelegateCommand(() =>
                {
                    if (_windowService.ShowDialog<CreateTaskViewModel>() == true)
                        UpdateTasks().Forget();
                }));
            }
        }

        public DelegateCommand<TaskViewModel> OpenTaskSessionsCommand
        {
            get
            {
                return _openTaskSessionsCommand ?? (_openTaskSessionsCommand = new DelegateCommand<TaskViewModel>(async parameter =>
                {
                    var sessions = await TasksResource.GetTaskSessions(parameter.Id, _restClient);
                    _windowService.ShowDialog<TaskOverviewViewModel>(x => x.Initialize(sessions, parameter));
                }));
            }
        }

        public DelegateCommand<TaskViewModel> TriggerCommand
        {
            get
            {
                return _triggerCommand ?? (_triggerCommand = new DelegateCommand<TaskViewModel>(async parameter =>
                {
                    await TasksResource.TriggerTask(parameter.Id, _restClient).OnErrorShowMessageBox(_windowService);
                }));
            }
        }

        public DelegateCommand<TaskViewModel> RemoveCommand
        {
            get
            {
                return _removeCommand ?? (_removeCommand = new DelegateCommand<TaskViewModel>(async parameter =>
                {
                    if (_windowService.ShowMessage(Tx.T("TasksInfrastructure:TaskOverview.RemoveTask", "name", parameter.Name), Tx.T("Warning"),
                            MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel) != MessageBoxResult.OK)
                        return;

                    await TasksResource.RemoveTask(parameter.Id, _restClient).OnErrorShowMessageBox(_windowService);
                }));
            }
        }

        private DelegateCommand<TaskViewModel> _toggleTaskIsEnabledCommand;

        public DelegateCommand<TaskViewModel> ToggleTaskIsEnabledCommand
        {
            get
            {
                return _toggleTaskIsEnabledCommand ?? (_toggleTaskIsEnabledCommand = new DelegateCommand<TaskViewModel>(parameter =>
                {
                    //TODO
                }));
            }
        }

        public override void OnActivated()
        {
            base.OnActivated();

            if (!_isTasksLoaded)
            {
                _isTasksLoaded = true;
                UpdateTasks().Forget();
            }
        }

        private async Task UpdateTasks()
        {
            var tasks = await TasksResource.GetTasks(_restClient);
            _tasks = new ObservableCollection<TaskViewModel>(tasks.Select(x => new TaskViewModel(x)));

            TasksView = new ListCollectionView(_tasks);
        }

        private void OnTaskExecutionCreated(TaskExecutionDto obj)
        {
            _dispatcher.Current.BeginInvoke(new Action(() =>
            {
                foreach (var taskViewModel in _tasks)
                {
                    if (taskViewModel.Sessions.Contains(obj.TaskSessionId))
                    {
                        taskViewModel.TotalExecutions++;
                        break;
                    }
                }
            }));
        }

        private void OnTaskSessionCreated(TaskSessionDto obj)
        {
            _dispatcher.Current.BeginInvoke(new Action(() =>
            {
                var taskViewModel = _tasks.FirstOrDefault(x => x.Id == obj.TaskReferenceId);
                taskViewModel?.Sessions.Add(obj.TaskSessionId);
            }));
        }
    }
}