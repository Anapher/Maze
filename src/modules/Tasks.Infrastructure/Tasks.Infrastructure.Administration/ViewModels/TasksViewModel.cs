using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Anapher.Wpf.Toolkit.Extensions;
using MahApps.Metro.IconPacks;
using Microsoft.AspNetCore.SignalR.Client;
using Maze.Administration.Library.Clients;
using Maze.Administration.Library.Extensions;
using Maze.Administration.Library.Services;
using Maze.Administration.Library.ViewModels;
using Maze.Server.Connection.Utilities;
using Maze.Utilities;
using Prism.Commands;
using Tasks.Infrastructure.Administration.Core;
using Tasks.Infrastructure.Administration.Rest.V1;
using Tasks.Infrastructure.Administration.ViewModels.Overview;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Core.Dtos;
using Unclassified.TxLib;
using Anapher.Wpf.Toolkit.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace Tasks.Infrastructure.Administration.ViewModels
{
    public class TasksViewModel : OverviewTabBase, IDisposable
    {
        private readonly IMazeRestClient _restClient;
        private readonly IAppDispatcher _dispatcher;
        private readonly IServiceProvider _services;
        private readonly IWindowService _windowService;
        private DelegateCommand _createTaskCommand;
        private bool _isTasksLoaded;
        private ObservableCollection<TaskViewModel> _tasks;
        private ICollectionView _tasksView;
        private DelegateCommand<TaskViewModel> _openTaskSessionsCommand;
        private DelegateCommand<TaskViewModel> _removeCommand;
        private DelegateCommand<TaskViewModel> _triggerCommand;
        private DelegateCommand<TaskViewModel> _toggleTaskIsEnabledCommand;
        private DelegateCommand<TaskViewModel> _updateTaskCommand;
        private readonly Stack<IDisposable> _disposables = new Stack<IDisposable>();
        private bool _isEventsInitialized;
        private DelegateCommand<PendingCommandViewModel> _openExecutionOverviewCommand;

        public TasksViewModel(IWindowService windowService, IMazeRestClient restClient, IAppDispatcher dispatcher, IServiceProvider services,
            CommandExecutionManager commandExecutionManager) : base(Tx.T("TasksInfrastructure:Tasks"), PackIconFontAwesomeKind.CalendarCheckRegular)
        {
            CommandExecutionManager = commandExecutionManager;
            _windowService = windowService;
            _restClient = restClient;
            _dispatcher = dispatcher;
            _services = services;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        public CommandExecutionManager CommandExecutionManager { get; }

        public ICollectionView TasksView
        {
            get => _tasksView;
            set => SetProperty(ref _tasksView, value);
        }

        public DelegateCommand CreateTaskCommand
        {
            get
            {
                return _createTaskCommand ?? (_createTaskCommand = new DelegateCommand(() => { _windowService.ShowDialog<CreateTaskViewModel>(); }));
            }
        }

        public DelegateCommand<TaskViewModel> OpenTaskSessionsCommand
        {
            get
            {
                return _openTaskSessionsCommand ?? (_openTaskSessionsCommand = new DelegateCommand<TaskViewModel>(async parameter =>
                {
                    var sessions = await TasksResource.GetTaskSessions(parameter.Id, _restClient);
                    using (var watcher = _services.GetRequiredService<TaskActivityWatcher>())
                    {
                        watcher.InitializeWatch(parameter.Id, sessions);

                        _windowService.ShowDialog<TaskOverviewViewModel>(x => x.Initialize(watcher, parameter.Name));
                    }
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

        public DelegateCommand<TaskViewModel> ToggleTaskIsEnabledCommand
        {
            get
            {
                return _toggleTaskIsEnabledCommand ?? (_toggleTaskIsEnabledCommand = new DelegateCommand<TaskViewModel>(async parameter =>
                {
                    if (parameter.IsEnabled)
                    {
                        await TasksResource.DisableTask(parameter.Id, _restClient).OnErrorShowMessageBox(_windowService);
                    }
                    else
                    {
                        await TasksResource.EnableTask(parameter.Id, _restClient).OnErrorShowMessageBox(_windowService);
                    }
                }));
            }
        }

        public DelegateCommand<TaskViewModel> UpdateTaskCommand
        {
            get
            {
                return _updateTaskCommand ?? (_updateTaskCommand = new DelegateCommand<TaskViewModel>(async parameter =>
                {
                    var resolver = _services.GetRequiredService<ITaskComponentResolver>();
                    var xmlCache = _services.GetRequiredService<IXmlSerializerCache>();

                    var task = await TasksResource.FetchTaskAsync(parameter.Id, resolver, xmlCache, _restClient);
                    _windowService.ShowDialog<CreateTaskViewModel>(vm => vm.UpdateTask(task));
                }));
            }
        }

        public DelegateCommand<PendingCommandViewModel> OpenExecutionOverviewCommand
        {
            get
            {
                return _openExecutionOverviewCommand ?? (_openExecutionOverviewCommand = new DelegateCommand<PendingCommandViewModel>(parameter =>
                {
                    _windowService.ShowDialog<TaskOverviewViewModel>(vm =>
                        vm.Initialize(parameter.TaskActivityWatcher, parameter.CommandDescription.Name));
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

            InitializeEvents();

            TasksView = new ListCollectionView(_tasks);
        }

        private void InitializeEvents()
        {
            if (_isEventsInitialized)
                return;

            _restClient.HubConnection.On<TaskExecutionDto>(HubEventNames.TaskExecutionCreated, OnTaskExecutionCreated).DisposeWith(_disposables);
            _restClient.HubConnection.On<TaskSessionDto>(HubEventNames.TaskSessionCreated, OnTaskSessionCreated).DisposeWith(_disposables);
            _restClient.HubConnection.On<Guid>(HubEventNames.TaskUpdated, OnTaskUpdated).DisposeWith(_disposables);
            _restClient.HubConnection.On<Guid>(HubEventNames.TaskRemoved, OnTaskRemoved).DisposeWith(_disposables);
            _restClient.HubConnection.On<Guid>(HubEventNames.TaskCreated, OnTaskCreated).DisposeWith(_disposables);

            _isEventsInitialized = true;
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

        private void OnTaskRemoved(Guid taskId)
        {
            _dispatcher.Current.BeginInvoke(new Action(() =>
            {
                var taskViewModel = _tasks.FirstOrDefault(x => x.Id == taskId);
                if (taskViewModel != null)
                    _tasks.Remove(taskViewModel);
            }));
        }

        private void OnTaskUpdated(Guid taskId)
        {
            _dispatcher.Current.BeginInvoke(new Action(async () =>
            {
                var taskViewModel = _tasks.FirstOrDefault(x => x.Id == taskId);
                if (taskViewModel != null)
                {
                    var taskInfo = await TasksResource.GetTaskInfo(taskId, _restClient);
                    taskViewModel.Update(taskInfo);
                }
            }));
        }

        private void OnTaskCreated(Guid taskId)
        {
            _dispatcher.Current.BeginInvoke(new Action(async () =>
            {
                var taskViewModel = _tasks.FirstOrDefault(x => x.Id == taskId);
                if (taskViewModel == null)
                {
                    var taskInfo = await TasksResource.GetTaskInfo(taskId, _restClient);
                    _tasks.Add(new TaskViewModel(taskInfo));
                }
            }));
        }
    }
}