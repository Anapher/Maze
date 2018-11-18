using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Microsoft.AspNetCore.SignalR.Client;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Services;
using Prism.Mvvm;
using Tasks.Infrastructure.Administration.ViewModels.Overview;
using Tasks.Infrastructure.Administration.ViewModels.TaskOverview;
using Tasks.Infrastructure.Core.Dtos;
using Unclassified.TxLib;

namespace Tasks.Infrastructure.Administration.ViewModels
{
    public class TaskOverviewViewModel : BindableBase
    {
        private readonly IAppDispatcher _dispatcher;
        private ObservableCollection<TaskSessionViewModel> _sessionsCollection;
        private Dictionary<string, TaskSessionViewModel> _sessions;
        private Dictionary<Guid, TaskExecutionViewModel> _executions;
        private Dictionary<Guid, ICommandStatusViewModel> _results;
        private object _selectedItem;

        public TaskOverviewViewModel(IOrcusRestClient restClient, IAppDispatcher dispatcher)
        {
            _dispatcher = dispatcher;

            restClient.HubConnection.On<TaskSessionDto>("TaskSessionCreated", OnTaskSessionCreated);
            restClient.HubConnection.On<TaskExecutionDto>("TaskExecutionCreated", OnTaskExecutionCreated);
            restClient.HubConnection.On<CommandResultDto>("TaskCommandResultCreated", OnTaskCommandResultCreated);
            restClient.HubConnection.On<CommandProcessDto>("TaskCommandProcess", OnTaskCommandProcess);
        }

        public string Title { get; private set; }

        public ICollectionView Sessions { get; private set; }

        public object SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        public void Initialize(TaskSessionsInfo taskSessions, TaskViewModel taskViewModel)
        {
            _sessions = taskSessions.Sessions.ToDictionary(x => x.TaskSessionId, dto => new TaskSessionViewModel(dto),
                StringComparer.OrdinalIgnoreCase);
            _executions = taskSessions.Executions.ToDictionary(x => x.TaskExecutionId, dto => new TaskExecutionViewModel(dto));
            _results = taskSessions.Results.ToDictionary(x => x.CommandResultId, dto => (ICommandStatusViewModel) new CommandResultViewModel(dto));

            var sessions = new ObservableCollection<TaskSessionViewModel>(_sessions.Values);

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

            _sessionsCollection = sessions;
            Sessions = new ListCollectionView(_sessionsCollection);

            Title = Tx.T("TasksInfrastructure:TaskOverview.Title", "name", taskViewModel.Name);
        }

        private void OnTaskSessionCreated(TaskSessionDto obj)
        {
            _dispatcher.Current.BeginInvoke(new Action(() =>
            {
                if (_sessions.ContainsKey(obj.TaskSessionId))
                    return;

                var viewModel = new TaskSessionViewModel(obj);
                _sessions.Add(obj.TaskSessionId, viewModel);
                _sessionsCollection.Add(viewModel);
            }));
        }

        private void OnTaskExecutionCreated(TaskExecutionDto obj)
        {
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
