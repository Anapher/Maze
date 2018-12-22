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
using Orcus.Server.Connection.Utilities;
using Orcus.Server.Library.Services;
using Orcus.Utilities;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Management;
using Tasks.Infrastructure.Management.Data;
using Tasks.Infrastructure.Server.Business;
using Tasks.Infrastructure.Server.Business.TaskManager;
using Tasks.Infrastructure.Server.Filter;
using Tasks.Infrastructure.Server.Library;
using Tasks.Infrastructure.Server.Rest;

namespace Tasks.Infrastructure.Server.Core
{
    public interface IOrcusTaskManager
    {
        /// <summary>
        ///     The tasks that must be executed at client level
        /// </summary>
        IImmutableList<TaskInfo> ClientTasks { get; }

        /// <summary>
        ///     The tasks that are currently executing on the server (or awaiting a trigger)
        /// </summary>
        IImmutableList<TaskInfo> LocalActiveTasks { get; }

        /// <summary>
        ///     Iniitalize the task manager. This must only be called once at startup.
        /// </summary>
        Task Initialize();
    }

    public interface IOrcusTaskManagerManagement
    {
        Task<TaskInfo> CancelTask(Guid taskId);
        Task InitializeTask(OrcusTask orcusTask, Hash hash, bool transmit, bool executeLocally);
        Task TriggerNow(OrcusTask task);
    }

    public class OrcusTaskManager : IOrcusTaskManager, IOrcusTaskManagerManagement
    {
        private readonly ITaskDirectory _taskDirectory;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OrcusTaskManager> _logger;
        private readonly Dictionary<Guid, TaskInfo> _tasks;
        private readonly AsyncLock _tasksLock;
        private IImmutableList<TaskInfo> _localActiveTasks;

        public OrcusTaskManager(ITaskDirectory taskDirectory, IServiceProvider serviceProvider,
            ILogger<OrcusTaskManager> logger)
        {
            _taskDirectory = taskDirectory;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _tasks = new Dictionary<Guid, TaskInfo>();
            ClientTasks = ImmutableList<TaskInfo>.Empty;
            LocalActiveTasks = ImmutableList<TaskInfo>.Empty;
            _tasksLock = new AsyncLock();
        }

        public IImmutableList<TaskInfo> ClientTasks { get; private set; }

        public IImmutableList<TaskInfo> LocalActiveTasks
        {
            get => _localActiveTasks;
            private set => _localActiveTasks = value;
        }
        
        public async Task Initialize()
        {
            using (await _tasksLock.LockAsync())
            {
                _logger.LogDebug("Initialize: Load tasks");
                var tasks = await _taskDirectory.LoadTasksRefresh();

                using (var scope = _serviceProvider.CreateScope())
                {
                    foreach (var task in tasks)
                    {
                        _logger.LogDebug("Initialize: Load task {taskId}", task.Id);
                        try
                        {
                            var action = scope.ServiceProvider.GetRequiredService<IVerifyTaskInDatabaseAction>();
                            var taskReference = await action.BizActionAsync(task);

                            if (action.HasErrors)
                            {
                                _logger.LogWarning("Initialize: Loading task {taskId} failed: {error}", task.Id, action.Errors.First().ErrorMessage);
                                continue;
                            }

                            if (!taskReference.IsEnabled)
                                continue;

                            var hash = _taskDirectory.ComputeTaskHash(task);

                            //transmit = false because that is executed on load and no clients should be connected anyways
                            InitializeTask(task, hash, transmit: false, executeLocally: !taskReference.IsCompleted).Forget();
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, "Error occurred when initializing task {taskId}", task.Id);
                        }
                    }
                }
            }
        }

        public async Task TriggerNow(OrcusTask task)
        {
            var service = new OrcusTaskService(task, _serviceProvider);
            await service.TriggerNow();
        }

        public async Task<TaskInfo> CancelTask(Guid taskId)
        {
            using (await _tasksLock.LockAsync())
            {
                if (_tasks.TryGetValue(taskId, out var taskInfo))
                {
                    taskInfo.Dispose();
                    _tasks.Remove(taskId);

                    ClientTasks = ClientTasks.Remove(taskInfo);

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var connectionManager = scope.ServiceProvider.GetRequiredService<IConnectionManager>();
                        foreach (var clientConnection in connectionManager.ClientConnections.Values)
                        {
                            try
                            {
                                await TasksResource.DeleteTask(taskId, clientConnection);
                            }
                            catch (Exception)
                            {
                                // ignored
                            }
                        }
                    }

                    return taskInfo;
                }

                throw new ArgumentException("The task could not be found.");
            }
        }

        public Task InitializeTask(OrcusTask orcusTask, Hash hash, bool transmit, bool executeLocally)
        {
            var (executeOnServer, executeOnClient) = GetTaskExecutionMode(orcusTask);

            var taskInfo = new TaskInfo(orcusTask, hash, executeOnServer, executeOnClient);
            if (!_tasks.TryAdd(orcusTask.Id, taskInfo))
            {
                taskInfo.Dispose();
                throw new InvalidOperationException($"Unable to add task with id {orcusTask.Id} because the task already exists.");
            }

            var executionTasks = new List<Task>();
            if (executeOnServer && executeLocally)
            {
                var taskService = new OrcusTaskService(orcusTask, _serviceProvider);
                taskService.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(OrcusTaskService.NextExecution))
                    {
                        taskInfo.NextExecution = taskService.NextExecution;
                        //Event
                    }
                };

                executionTasks.Add(RunTask(taskService, taskInfo.Token).ContinueWith(task => LocalTaskExecutionCompleted(taskInfo)));

                LocalActiveTasks = LocalActiveTasks.Add(taskInfo);
            }

            if (executeOnClient)
            {
                //we always transmit to clients because they have their own logic to check if it completed

                if (transmit)
                    executionTasks.Add(TransmitTask(orcusTask, taskInfo.Token));

                ClientTasks = ClientTasks.Add(taskInfo);
            }
            
            return Task.WhenAll(executionTasks).ContinueWith(task => TaskExecutionCompleted(taskInfo));
        }

        private async Task TransmitTask(OrcusTask orcusTask, CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;

                var filters = new AggregatedClientFilter(services.GetRequiredService<ILogger< AggregatedClientFilter>>());

                filters.Add(new AudienceFilter(orcusTask.Audience));
                foreach (var filterInfo in orcusTask.Filters)
                {
                    var filterType = typeof(IFilterService<>).MakeGenericType(filterInfo.GetType());
                    filters.Add(new CustomFilterFactory(filterType, filterInfo, services.GetRequiredService < ILogger<CustomFilterFactory>>()));
                }

                var tasksComponentResolver = services.GetRequiredService<ITaskComponentResolver>();
                var xmlSerializerCache = services.GetRequiredService<IXmlSerializerCache>();

                var connectionManager = services.GetRequiredService<IConnectionManager>();
                foreach (var clientConnection in connectionManager.ClientConnections.Values)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (!await filters.IsClientIncluded(clientConnection.ClientId, services))
                        continue;

                    try
                    {
                        await TasksResource.CreateOrUpdateTask(orcusTask, tasksComponentResolver, xmlSerializerCache, clientConnection);
                    }
                    catch (Exception e)
                    {
                        _logger.LogWarning(e, "Transmitting task {taskId} to client {clientId} failed.", orcusTask.Id, clientConnection.ClientId);
                        continue;
                    }

                    var action = services.GetRequiredService<ICreateTaskTransmissionAction>();
                    await action.BizActionAsync(new TaskTransmission
                    {
                        TaskReferenceId = orcusTask.Id, TargetId = clientConnection.ClientId, CreatedOn = DateTimeOffset.UtcNow
                    });
                }
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

            using (var scope = _serviceProvider.CreateScope())
            {
                var action = scope.ServiceProvider.GetRequiredService<IMarkTaskCompletedAction>();
                await action.BizActionAsync(taskService.OrcusTask.Id);
            }
        }

        private void LocalTaskExecutionCompleted(TaskInfo taskInfo)
        {
            // thread safe version of
            // LocalActiveTasks = LocalActiveTasks.Remove(taskInfo);
            while (true)
            {
                var localActiveTasks = LocalActiveTasks;
                var modified = localActiveTasks.Remove(taskInfo);

                if (Interlocked.CompareExchange(ref _localActiveTasks, modified, localActiveTasks) == modified)
                    break;
            }
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