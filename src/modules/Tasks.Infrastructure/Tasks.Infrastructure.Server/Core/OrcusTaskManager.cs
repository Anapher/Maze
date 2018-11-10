using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using Orcus.Server.Connection;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Management;
using Tasks.Infrastructure.Server.Business;
using Tasks.Infrastructure.Server.Library;

namespace Tasks.Infrastructure.Server.Core
{
    public class OrcusTaskManager
    {
        private readonly ITaskDirectory _taskDirectory;
        private readonly ITasksConnectionManager _connectionManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OrcusTaskManager> _logger;
        private readonly Dictionary<Guid, TaskInfo> _tasks;
        private readonly AsyncLock _tasksLock;

        public OrcusTaskManager(ITaskDirectory taskDirectory, ITasksConnectionManager connectionManager, IServiceProvider serviceProvider,
            ILogger<OrcusTaskManager> logger)
        {
            _taskDirectory = taskDirectory;
            _connectionManager = connectionManager;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _tasks = new Dictionary<Guid, TaskInfo>();
            ClientTasks = ImmutableList<TaskInfo>.Empty;
            _tasksLock = new AsyncLock();
        }

        public IImmutableList<TaskInfo> ClientTasks { get; private set; }

        public async Task Initialize()
        {
            using (await _tasksLock.LockAsync())
            {
                _logger.LogDebug("Initialize: Load tasks");
                var tasks = await _taskDirectory.LoadTasks();

                using (var scope = _serviceProvider.CreateScope())
                {
                    foreach (var task in tasks)
                    {
                        _logger.LogDebug("Initialize: Load task {taskId}", task.Id);

                        var action = scope.ServiceProvider.GetRequiredService<ICreateTaskAction>();
                        await action.BizActionAsync(task);

                        if (action.HasErrors)
                        {
                            _logger.LogWarning("Initialize: Loading task {taskId} failed: {error}", task.Id, action.Errors.First().ErrorMessage);
                            continue;
                        }

                        var hash = _taskDirectory.ComputeTaskHash(task);
                        InitializeTask(task, hash, transmit: false);
                    }
                }
            }
        }

        public async Task AddTask(OrcusTask orcusTask)
        {
            using (await _tasksLock.LockAsync())
            {
                var hash = _taskDirectory.ComputeTaskHash(orcusTask);

                if (_tasks.TryGetValue(orcusTask.Id, out var taskInfo))
                {
                    if (hash.Equals(taskInfo.Hash)) //the tasks are equal
                        return;

                    throw new InvalidOperationException($"The task with id {orcusTask.Id} already exists. Please update the existing task.");
                }

                await _taskDirectory.WriteTask(orcusTask);

                //add to database
                using (var scope = _serviceProvider.CreateScope())
                {
                    var action = scope.ServiceProvider.GetRequiredService<ICreateTaskAction>();
                    await action.BizActionAsync(orcusTask);

                    if (action.HasErrors)
                        throw new InvalidOperationException(action.Errors.First().ErrorMessage);
                }

                InitializeTask(orcusTask, hash, transmit: true);
            }
        }

        private void InitializeTask(OrcusTask orcusTask, Hash hash, bool transmit)
        {
            var (executeOnServer, executeOnClient) = GetTaskExecutionMode(orcusTask);

            var taskInfo = new TaskInfo(orcusTask, hash, executeOnServer, executeOnClient);
            if (!_tasks.TryAdd(orcusTask.Id, taskInfo))
            {
                taskInfo.Dispose();
                throw new InvalidOperationException($"Unable to add task with id {orcusTask.Id} because the task already exists.");
            }

            var executionTasks = new List<Task>();
            if (executeOnServer)
            {
                var taskService = new OrcusTaskService(orcusTask, _serviceProvider);
                executionTasks.Add(RunTask(taskService, taskInfo.Token));
            }
            if (executeOnClient)
            {
                if (transmit)
                    executionTasks.Add(TransmitTask(orcusTask, taskInfo.Token));

                ClientTasks = ClientTasks.Add(taskInfo);
            }

            Task.WhenAll(executionTasks).ContinueWith(task => TaskExecutionCompleted(taskInfo));
        }

        private async Task TransmitTask(OrcusTask orcusTask, CancellationToken cancellationToken)
        {
            foreach (var clientInfo in _connectionManager.Clients)
            {
                await (await clientInfo.Value.GetChannel()).CreateOrUpdateTask("");
            }
        }

        private async Task RunTask(OrcusTaskService taskService, CancellationToken cancellationToken)
        {
            try
            {
                await taskService.Run(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return; //just ignore
            }
            catch (Exception e)
            {
                taskService.Logger.LogCritical(e, "An error occurred when running task service for task {taskId}.", taskService.OrcusTask.Id);
                return;
            }

            //set finished
        }

        private void TaskExecutionCompleted(TaskInfo taskInfo)
        {
            taskInfo.Dispose();
        }

        private (bool server, bool client) GetTaskExecutionMode(OrcusTask orcusTask)
        {
            var executeOnServer = false;
            var executeOnClient = false;

            using (var scope = _serviceProvider.CreateScope())
            {
                foreach (var orcusTaskCommand in orcusTask.Commands)
                {
                    var serviceType = typeof(ITaskExecutor<>).MakeGenericType(orcusTaskCommand.GetType());
                    var service = scope.ServiceProvider.GetService(serviceType);
                    if (service == null)
                        executeOnClient = true;
                    else executeOnServer = true;
                }
            }

            return (executeOnServer, executeOnClient);
        }
    }
}