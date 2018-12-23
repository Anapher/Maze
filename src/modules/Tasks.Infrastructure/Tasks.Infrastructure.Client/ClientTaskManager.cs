using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orcus.Client.Library.Clients;
using Orcus.Server.Connection;
using Orcus.Server.Connection.Utilities;
using Orcus.Utilities;
using Tasks.Infrastructure.Client.Library;
using Tasks.Infrastructure.Client.Rest.V1;
using Tasks.Infrastructure.Client.Storage;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Core.Dtos;
using Tasks.Infrastructure.Management;

namespace Tasks.Infrastructure.Client
{
    public interface IClientTaskManager
    {
        ConcurrentDictionary<Guid, (TaskRunner, CancellationTokenSource)> Tasks { get; }
        Task Initialize();
        Task RemoveTask(Guid taskId);
        Task Synchronize(List<TaskSyncDto> tasks, IRestClient restClient);
        Task AddOrUpdateTask(OrcusTask orcusTask);
        Task TriggerNow(OrcusTask task, SessionKey sessionKey, ITaskStorage taskStorage);
    }

    public class ClientTaskManager : IClientTaskManager
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ITaskDirectory _taskDirectory;
        private readonly ILogger<ClientTaskManager> _logger;
        private readonly object _taskUpdateLock = new object();
        private readonly IDatabaseTaskStorage _taskStorage;

        public ClientTaskManager(ITaskDirectory taskDirectory, IDatabaseTaskStorage taskStorage, IServiceProvider serviceProvider, ILogger<ClientTaskManager> logger)
        {
            _taskDirectory = taskDirectory;
            _taskStorage = taskStorage;
            _serviceProvider = serviceProvider;
            _logger = logger;

            Tasks = new ConcurrentDictionary<Guid, (TaskRunner, CancellationTokenSource)>();
        }

        public ConcurrentDictionary<Guid, (TaskRunner, CancellationTokenSource)> Tasks { get; }

        public async Task Initialize()
        {
            _logger.LogDebug("Initialize tasks");
            
            var tasks = await _taskDirectory.LoadTasksRefresh();
            foreach (var orcusTask in tasks)
            {
                if (await _taskStorage.CheckTaskFinished(orcusTask))
                    continue;

                var taskRunner = new TaskRunner(orcusTask, _taskStorage, _serviceProvider);
                var cancellationTokenSource = new CancellationTokenSource();

                Tasks.TryAdd(orcusTask.Id, (taskRunner, cancellationTokenSource));
                RunTask(taskRunner, cancellationTokenSource.Token).ContinueWith(_ => cancellationTokenSource.Dispose()).Forget();
            }
        }

        public async Task RemoveTask(Guid taskId)
        {
            if (Tasks.TryGetValue(taskId, out var taskInfo))
            {
                taskInfo.Item2.Cancel();
                await _taskDirectory.RemoveTask(taskInfo.Item1.OrcusTask.Id);
            }
            else
            {
                foreach (var task in await _taskDirectory.LoadTasks())
                {
                    if (task.Id == taskId)
                        await _taskDirectory.RemoveTask(task.Id);
                }
            }
        }

        public async Task Synchronize(List<TaskSyncDto> tasks, IRestClient restClient)
        {
            _logger.LogDebug("Synchronize tasks...");

            var tasksToDelete = (await _taskDirectory.LoadTasks()).ToDictionary(x => x.Id, x => _taskDirectory.ComputeTaskHash(x));
            var tasksToUpdate = new List<TaskSyncDto>();

            foreach (var taskSyncDto in tasks)
            {
                if (tasksToDelete.TryGetValue(taskSyncDto.TaskId, out var taskHash))
                {
                    if (taskHash.Equals(Hash.Parse(taskSyncDto.Hash)))
                    {
                        //the hash values match, this task is fine
                        tasksToDelete.Remove(taskSyncDto.TaskId);
                        continue;
                    }
                }

                //the task was either not found or the hash values don't match
                tasksToUpdate.Add(taskSyncDto);
            }

            if (tasksToUpdate.Any())
            {
                var taskComponentResolver = _serviceProvider.GetRequiredService<ITaskComponentResolver>();
                var xmlSerializerCache = _serviceProvider.GetRequiredService<IXmlSerializerCache>();

                await TaskCombinators.ThrottledAsync(tasksToUpdate, async (dto, token) =>
                {
                    _logger.LogDebug("Update task {taskId}", dto.TaskId);
                    try
                    {
                        var orcusTask = await TasksResource.FetchTaskAsync(dto.TaskId, taskComponentResolver, xmlSerializerCache, restClient);
                        await AddOrUpdateTask(orcusTask);
                    }
                    catch (Exception e)
                    {
                        _logger.LogWarning(e, "An error occurred when trying to run task {taskId}", dto.TaskId);
                    }
                }, CancellationToken.None);
            }

            await UpdateTaskMachineStatus(restClient);
        }

        public async Task AddOrUpdateTask(OrcusTask orcusTask)
        {
            await _taskDirectory.WriteTask(orcusTask);

            if (Tasks.TryGetValue(orcusTask.Id, out var existingTaskInfo))
            {
                _logger.LogDebug("Cancel runner for task {taskId}", orcusTask.Id);
                existingTaskInfo.Item2.Cancel();
                Tasks.TryRemove(orcusTask.Id, out _);
            }

            var taskRunner = new TaskRunner(orcusTask, _taskStorage, _serviceProvider);
            var cancellationTokenSource = new CancellationTokenSource();

            lock (_taskUpdateLock)
            {
                Tasks.TryRemove(orcusTask.Id, out _);
                Tasks.TryAdd(orcusTask.Id, (taskRunner, cancellationTokenSource));
            }

            RunTask(taskRunner, cancellationTokenSource.Token).ContinueWith(_ => cancellationTokenSource.Dispose()).Forget();
        }

        public async Task TriggerNow(OrcusTask task, SessionKey sessionKey, ITaskStorage taskStorage)
        {
            var taskRunner = new TaskRunner(task, taskStorage, _serviceProvider);
            await taskRunner.TriggerNow(sessionKey);
        }

        private async Task RunTask(TaskRunner taskRunner, CancellationToken cancellationToken)
        {
            taskRunner.PropertyChanged += TaskRunnerOnPropertyChanged;

            try
            {
                await taskRunner.Run(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                //when the app is shutting down or when the task should be removed
                return;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred when starting task runner for task {taskId}", taskRunner.OrcusTask.Id);
            }
            finally
            {
                lock (_taskUpdateLock)
                {
                    if (Tasks.TryRemove(taskRunner.OrcusTask.Id, out var taskInfo) && taskInfo.Item1 != taskRunner)
                        Tasks.TryAdd(taskRunner.OrcusTask.Id, taskInfo);
                }
            }
            
            //the task is marked as finished in the task runner

            await TryUpdateTaskMachineStatus();
        }

        private void TaskRunnerOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(TaskRunner.NextTrigger):
                    TryUpdateTaskMachineStatus().Forget();
                    break;
            }
        }

        private async Task TryUpdateTaskMachineStatus()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var restClient = scope.ServiceProvider.GetRequiredService<IRestClient>();
                await UpdateTaskMachineStatus(restClient);
            }
        }

        private async Task UpdateTaskMachineStatus(IRestClient restClient)
        {
            var tasks = (await _taskDirectory.LoadTasks()).Select(x =>
            {
                if (Tasks.TryGetValue(x.Id, out var info))
                    return new ClientTaskDto {TaskId = x.Id, NextTrigger = info.Item1.NextTrigger, IsActive = true};

                return new ClientTaskDto {TaskId = x.Id};
            }).ToList();

            try
            {
                await TasksResource.UpdateMachineStatus(tasks, restClient);
            }
            catch (Exception)
            {
                // doesn't matter
            }
        }
    }
}