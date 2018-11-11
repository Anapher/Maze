using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orcus.Client.Library.Clients;
using Orcus.Server.Connection;
using Orcus.Server.Connection.Utilities;
using Orcus.Utilities;
using Tasks.Infrastructure.Client.Rest;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Core.Dtos;
using Tasks.Infrastructure.Management;

namespace Tasks.Infrastructure.Client
{
    public class ClientTaskManager
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ITaskDirectory _taskDirectory;
        private readonly ILogger<ClientTaskManager> _logger;
        private readonly object _taskUpdateLock = new object();

        public ClientTaskManager(ITaskDirectory taskDirectory, IServiceProvider serviceProvider, ILogger<ClientTaskManager> logger)
        {
            _taskDirectory = taskDirectory;
            _serviceProvider = serviceProvider;
            _logger = logger;

            Tasks = new ConcurrentDictionary<Guid, (TaskRunner, CancellationTokenSource)>();
        }

        public ConcurrentDictionary<Guid, (TaskRunner, CancellationTokenSource)> Tasks { get; }

        public async Task Initialize()
        {
            _logger.LogDebug("Initialize tasks");

            var sessionManager = _serviceProvider.GetRequiredService<ITaskSessionManager>();

            var tasks = await _taskDirectory.LoadTasksRefresh();
            foreach (var orcusTask in tasks)
            {
                if (await sessionManager.CheckTaskFinished(orcusTask))
                    continue;

                var taskRunner = new TaskRunner(orcusTask, _serviceProvider);
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
                await _taskDirectory.RemoveTask(taskInfo.Item1.OrcusTask);
            }
            else
            {
                foreach (var task in await _taskDirectory.LoadTasks())
                {
                    if (task.Id == taskId)
                        await _taskDirectory.RemoveTask(task);
                }
            }
        }

        public async Task Synchronize(List<TaskSyncDto> tasks, IRestClient restClient)
        {
            _logger.LogDebug("Synchronize tasks...");

            var tasksToDelete = Tasks.ToDictionary(x => x.Key, x => x.Value);
            var tasksToUpdate = new List<TaskSyncDto>();

            foreach (var taskSyncDto in tasks)
            {
                if (tasksToDelete.TryGetValue(taskSyncDto.TaskId, out var info))
                {
                    var localTaskHash = _taskDirectory.ComputeTaskHash(info.Item1.OrcusTask);
                    if (localTaskHash.Equals(Hash.Parse(taskSyncDto.Hash)))
                    {
                        //the hash values match, this task is fine
                        tasksToDelete.Remove(taskSyncDto.TaskId);
                        continue;
                    }
                }

                //the task was either not found or the hash values don't match
                tasksToUpdate.Add(taskSyncDto);
            }

            if (tasksToDelete.Any())
            {
                foreach (var (taskRunner, cancellationTokenSource) in tasksToDelete.Values)
                {
                    _logger.LogDebug("Remove task {taskId}", taskRunner.OrcusTask.Id);

                    //that will remove the task from the dictionary
                    cancellationTokenSource.Cancel();
                    await _taskDirectory.RemoveTask(taskRunner.OrcusTask);
                }
            }

            //remove tasks that are not listed anymore
            foreach (var task in await _taskDirectory.LoadTasks())
            {
                if (!tasks.Any(x => x.TaskId == task.Id))
                {
                    await _taskDirectory.RemoveTask(task);
                }
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

            var taskRunner = new TaskRunner(orcusTask, _serviceProvider);
            var cancellationTokenSource = new CancellationTokenSource();

            lock (_taskUpdateLock)
            {
                Tasks.TryRemove(orcusTask.Id, out _);
                Tasks.TryAdd(orcusTask.Id, (taskRunner, cancellationTokenSource));
            }

            RunTask(taskRunner, cancellationTokenSource.Token).ContinueWith(_ => cancellationTokenSource.Dispose()).Forget();
        }

        private async Task RunTask(TaskRunner taskRunner, CancellationToken cancellationToken)
        {
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
            
            await _taskDirectory.RemoveTask(taskRunner.OrcusTask);
        }
    }
}