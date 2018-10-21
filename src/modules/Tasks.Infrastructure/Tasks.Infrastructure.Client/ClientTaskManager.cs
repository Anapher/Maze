using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orcus.Client.Library.Clients;
using Orcus.Server.Connection;
using Orcus.Utilities;
using Tasks.Infrastructure.Core.Dtos;

namespace Tasks.Infrastructure.Client
{
    public class ClientTaskManager
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ITaskDirectory _taskDirectory;
        private readonly ILogger<ClientTaskManager> _logger;

        public ClientTaskManager(ITaskDirectory taskDirectory, IServiceProvider serviceProvider, ILogger<ClientTaskManager> logger)
        {
            _taskDirectory = taskDirectory;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public IImmutableDictionary<Guid, (TaskRunner, CancellationTokenSource)> Tasks { get; private set; }

        public async Task Initialize()
        {
            _logger.LogDebug("Initialize tasks");

            var tasks = await _taskDirectory.LoadTasks();
            var tasksList = new Dictionary<Guid, (TaskRunner, CancellationTokenSource)>();

            foreach (var orcusTask in tasks)
            {
                var taskRunner = new TaskRunner(orcusTask, _serviceProvider);
                var cancellationTokenSource = new CancellationTokenSource();

                tasksList.Add(orcusTask.Id, (taskRunner, cancellationTokenSource));
            }

            Tasks = tasksList.ToImmutableDictionary();

            foreach (var (taskRunner, cancellationTokenSource) in tasksList.Values)
            {
                RunTask(taskRunner, cancellationTokenSource.Token).ContinueWith(_ => cancellationTokenSource.Dispose()).Forget();
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
                        tasksToDelete.Remove(taskSyncDto.TaskId);
                        continue;
                    }
                }

                tasksToUpdate.Add(taskSyncDto);
            }


        }

        private async Task RunTask(TaskRunner taskRunner, CancellationToken cancellationToken)
        {
            try
            {
                await taskRunner.Run(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred when starting task runner for task {taskId}", taskRunner.OrcusTask.Id);
            }

            Tasks = Tasks.Remove(taskRunner.OrcusTask.Id);
            await _taskDirectory.RemoveTask(taskRunner.OrcusTask);
        }
    }
}