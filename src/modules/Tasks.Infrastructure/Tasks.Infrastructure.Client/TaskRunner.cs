using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orcus.Utilities;
using Tasks.Infrastructure.Client.Library;
using Tasks.Infrastructure.Client.StopEvents;
using Tasks.Infrastructure.Client.Trigger;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Core.Data;
using Tasks.Infrastructure.Management;

namespace Tasks.Infrastructure.Client
{
    public class TaskRunner
    {
        public TaskRunner(OrcusTask orcusTask, IServiceProvider services)
        {
            OrcusTask = orcusTask;
            Services = services;
            Logger = services.GetRequiredService<ILogger<TaskRunner>>();
        }

        public OrcusTask OrcusTask { get; }
        public IServiceProvider Services { get; }
        public ILogger Logger { get; }

        public async Task Run(CancellationToken cancellationToken)
        {
            using (var localCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            using (var scope = Services.CreateScope())
            {
                //create stop services and add to dictionary
                var stopTasks = new Dictionary<Task, Type>();
                foreach (var stopEvent in OrcusTask.StopEvents)
                {
                    var serviceType = typeof(IStopService<>).MakeGenericType(stopEvent.GetType());

                    var service = scope.ServiceProvider.GetService(serviceType);
                    if (service == null)
                    {
                        Logger.LogWarning("The stop service for type {stopEvent} ({resolvedType}) could not be resolved. Skipped.",
                            stopEvent.GetType(), serviceType);
                        continue;
                    }

                    var stopContext = new DefaultStopContext();
                    var methodInfo = serviceType.GetMethod("InvokeAsync", BindingFlags.Instance);

                    try
                    {
                        var task = (Task) methodInfo.Invoke(service, new object[] {stopEvent, stopContext, localCancellationTokenSource.Token});
                        stopTasks.Add(task, serviceType);
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e, "Error occurred when invoking stop service {stopServiceType}", serviceType);
                    }
                }

                if (stopTasks.Any())
                {
                    //if we have any stop events, we await until one completes.
                    Task.WhenAny(stopTasks.Keys).ContinueWith(task =>
                    {
                        if (task.IsCanceled)
                            return;

                        var type = stopTasks[task.Result];

                        if (task.IsFaulted) //stop the execution even if it fails, because else the task might run endless
                            Logger.LogError(task.Exception, "An error occurred on execution {stopService} on task {task}", type, OrcusTask.Id);

                        Logger.LogDebug("Stop service {stopService} stopped the execution of task {task}", type, OrcusTask.Id);
                        localCancellationTokenSource.Cancel();
                    }).Forget();
                }

                //create triggers and add to dictionary
                var triggerTasks = new Dictionary<Task, Type>();
                foreach (var triggerInfo in OrcusTask.Triggers)
                {
                    var serviceType = typeof(ITriggerService<>).MakeGenericType(triggerInfo.GetType());

                    var service = scope.ServiceProvider.GetService(serviceType);
                    if (service == null)
                    {
                        Logger.LogWarning("The trigger service for type {triggerInfo} ({resolvedType}) could not be resolved. Skipped.",
                            triggerInfo.GetType(), serviceType);
                        continue;
                    }

                    var triggerContext = new TaskTriggerContext(this, serviceType.Name);
                    var methodInfo = serviceType.GetMethod("InvokeAsync", BindingFlags.Instance);

                    try
                    {
                        var task = (Task) methodInfo.Invoke(service, new object[] {triggerInfo, triggerContext, localCancellationTokenSource.Token});
                        triggerTasks.Add(task, serviceType);
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e, "Error occurred when invoking trigger service {triggerServiceType}", serviceType);
                    }
                }

                //wait until all triggers have finished
                while (triggerTasks.Any())
                {
                    var task = await Task.WhenAny(triggerTasks.Keys);
                    if (localCancellationTokenSource.IsCancellationRequested)
                        throw new TaskCanceledException();

                    if (task.IsFaulted)
                    {
                        var type = triggerTasks[task];
                        Logger.LogError(task.Exception, "An error occurred when awaiting the trigger {trigger}", type);
                    }

                    triggerTasks.Remove(task);
                }

                //also cancel on end of execution so the stop services can finish (e. g. on natual completion)
                localCancellationTokenSource.Cancel();
            }
        }

        internal async Task Execute(TaskSession taskSession, CancellationToken cancellationToken)
        {
            using (var executionScope = Services.CreateScope())
            {
                var context = new DefaultTaskExecutionContext(Services);

                await TaskCombinators.ThrottledAsync(OrcusTask.Commands, async (commandInfo, token) =>
                {
                    var executorType = typeof(ITaskExecutor<>).MakeGenericType(commandInfo.GetType());

                    var localService = executionScope.ServiceProvider.GetService(executorType);
                    if (localService != null)
                    {
                        var executionMethod = executorType.GetMethod("InvokeAsync", BindingFlags.Instance);
                        var commandName = commandInfo.GetType().Name.Replace("CommandInfo", null);

                        var execution = new TaskExecution
                        {
                            TaskSessionId = taskSession.TaskSessionId, Timestamp = DateTimeOffset.UtcNow, CommandName = commandName
                        };

                        try
                        {
                            var task = (Task<HttpResponseMessage>) executionMethod.Invoke(Services,
                                new object[] {commandInfo, context, cancellationToken});
                            var response = await task;

                            using (var memoryStream = new MemoryStream())
                            {
                                await HttpResponseSerializer.Format(response, memoryStream);

                                execution.Result = Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int) memoryStream.Length);
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.LogWarning(e, "An error occurred when executing {method}", executorType.FullName);
                            execution.Result = e.ToString();
                        }

                        var sessionManager = Services.GetRequiredService<ITaskSessionManager>();
                        await sessionManager.CreateExecution(OrcusTask, taskSession, execution);
                    }
                }, cancellationToken);
            }
        }
    }
}