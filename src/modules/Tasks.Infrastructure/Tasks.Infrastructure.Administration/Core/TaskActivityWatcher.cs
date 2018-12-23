using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.AspNetCore.SignalR.Client;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Services;
using Orcus.Server.Connection.Utilities;
using Tasks.Infrastructure.Administration.ViewModels.TaskOverview;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Core.Dtos;

namespace Tasks.Infrastructure.Administration.Core
{
    public class TaskActivityWatcher : IDisposable
    {
        private readonly IOrcusRestClient _restClient;
        private readonly IAppDispatcher _dispatcher;
        private Guid _taskId;
        private readonly Stack<IDisposable> _disposables = new Stack<IDisposable>();
        private Dictionary<string, TaskSessionViewModel> _sessions;
        private Dictionary<Guid, TaskExecutionViewModel> _executions;
        private Dictionary<Guid, ICommandStatusViewModel> _results;

        public TaskActivityWatcher(IOrcusRestClient restClient, IAppDispatcher dispatcher)
        {
            _restClient = restClient;
            _dispatcher = dispatcher;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        public ObservableCollection<TaskSessionViewModel> Sessions { get; private set; }

        public void InitializeData(Guid taskId, TaskSessionsInfo taskSessions)
        {
            _taskId = taskId;

            InitializeData(taskSessions);
        }

        public void InitializeWatch(Guid taskId, TaskSessionsInfo taskSessions)
        {
            _taskId = taskId;

            InitializeData(taskSessions);
            InitializeEvents();
        }

        public void InitializeWatch(Guid taskId)
        {
            _taskId = taskId;

            _sessions = new Dictionary<string, TaskSessionViewModel>();
            _executions = new Dictionary<Guid, TaskExecutionViewModel>();
            _results = new Dictionary<Guid, ICommandStatusViewModel>();

            Sessions = new ObservableCollection<TaskSessionViewModel>();

            InitializeEvents();
        }

        private void InitializeEvents()
        {
            _restClient.HubConnection.On<TaskSessionDto>(HubEventNames.TaskSessionCreated, OnTaskSessionCreated).DisposeWith(_disposables);
            _restClient.HubConnection.On<TaskExecutionDto>(HubEventNames.TaskExecutionCreated, OnTaskExecutionCreated).DisposeWith(_disposables);
            _restClient.HubConnection.On<CommandResultDto>(HubEventNames.TaskCommandResultCreated, OnTaskCommandResultCreated).DisposeWith(_disposables);
            _restClient.HubConnection.On<CommandProcessDto>(HubEventNames.TaskCommandProcess, OnTaskCommandProcess).DisposeWith(_disposables);
        }

        private void InitializeData(TaskSessionsInfo taskSessions)
        {
            _sessions = taskSessions.Sessions.ToDictionary(x => x.TaskSessionId, dto => new TaskSessionViewModel(dto),
                StringComparer.OrdinalIgnoreCase);
            _executions = taskSessions.Executions.ToDictionary(x => x.TaskExecutionId, dto => new TaskExecutionViewModel(dto));
            _results = taskSessions.Results.ToDictionary(x => x.CommandResultId, dto => (ICommandStatusViewModel)new CommandResultViewModel(dto));

            Sessions = new ObservableCollection<TaskSessionViewModel>(_sessions.Values);
            foreach (var executions in _executions.Values.GroupBy(x => x.TaskSessionId))
            {
                if (!_sessions.TryGetValue(executions.Key, out var sessionViewModel))
                    continue;

                sessionViewModel.Executions = new ObservableCollection<TaskExecutionViewModel>();
                foreach (var taskExecution in executions)
                {
                    var results = _results.Values.Where(x => x.TaskExecutionId == taskExecution.TaskExecutionId);
                    taskExecution.Results = new ObservableCollection<ICommandStatusViewModel>(results);

                    sessionViewModel.Executions.Add(taskExecution);
                }
            }
        }

        private void OnTaskSessionCreated(TaskSessionDto obj)
        {
            if (obj.TaskReferenceId != _taskId)
                return;

            _dispatcher.Current.BeginInvoke(new Action(() =>
            {
                if (_sessions.ContainsKey(obj.TaskSessionId))
                    return;

                var viewModel = new TaskSessionViewModel(obj);
                _sessions.Add(obj.TaskSessionId, viewModel);
                Sessions.Add(viewModel);
            }));
        }

        private void OnTaskExecutionCreated(TaskExecutionDto obj)
        {
            if (obj.TaskReferenceId != _taskId)
                return;

            _dispatcher.Current.BeginInvoke(new Action(() =>
            {
                if (!_sessions.TryGetValue(obj.TaskSessionId, out var taskSession))
                    return;

                if (taskSession.Executions == null)
                    taskSession.Executions = new ObservableCollection<TaskExecutionViewModel>();

                var execution = new TaskExecutionViewModel(obj);

                _executions.Add(execution.TaskExecutionId, execution);
                taskSession.Executions.Add(execution);
            }));
        }

        private void OnTaskCommandResultCreated(CommandResultDto obj)
        {
            _dispatcher.Current.BeginInvoke(new Action(() =>
            {
                if (!_executions.TryGetValue(obj.TaskExecutionId, out var execution))
                    return;

                if (execution.Results == null)
                    execution.Results = new ObservableCollection<ICommandStatusViewModel>();
                else
                {
                    var existingResult = execution.Results.FirstOrDefault(x => x.CommandResultId == obj.CommandResultId);
                    if (existingResult != null)
                    {
                        execution.Results.Remove(existingResult);
                        _results.Remove(existingResult.CommandResultId);
                    }
                }

                var result = new CommandResultViewModel(obj);

                _results.Add(result.CommandResultId, result);
                execution.Results.Add(result);
            }));
        }

        private void OnTaskCommandProcess(CommandProcessDto obj)
        {
            _dispatcher.Current.BeginInvoke(new Action(() =>
            {
                if (_results.TryGetValue(obj.CommandResultId, out var commandResult))
                {
                    if (commandResult is CommandProcessViewModel commandProcess)
                    {
                        commandProcess.Update(obj);
                    } //if command result exists, don't do anything

                    return;
                }

                if (_executions.TryGetValue(obj.TaskExecutionId, out var taskExecutionViewModel))
                {
                    var commandProcess = new CommandProcessViewModel(obj);

                    _results.Add(commandProcess.CommandResultId, commandProcess);

                    if (taskExecutionViewModel.Results == null)
                        taskExecutionViewModel.Results = new ObservableCollection<ICommandStatusViewModel>();
                    taskExecutionViewModel.Results.Add(commandProcess);
                }
            }));
        }
    }
}