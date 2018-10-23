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
            using (var scope = Services.CreateScope())
            {
                var tasks = new Dictionary<Task, Type>();

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
                        var task = (Task) methodInfo.Invoke(service, new object[] {triggerInfo, triggerContext, cancellationToken});
                        tasks.Add(task, serviceType);
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e, "Error occurred when invoking trigger service {triggerServiceType}", serviceType);
                    }

                    while (tasks.Any())
                    {
                        var task = await Task.WhenAny(tasks.Keys);
                        if (cancellationToken.IsCancellationRequested)
                            throw new TaskCanceledException();

                        if (task.IsFaulted)
                        {
                            var type = tasks[task];
                            Logger.LogError(task.Exception, "An error occurred when awaiting the trigger {trigger}", type);
                        }

                        tasks.Remove(task);
                    }
                }
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
                        Stream bodyStream = null;

                        try
                        {
                            var task = (Task<HttpResponseMessage>) executionMethod.Invoke(Services,
                                new object[] {commandInfo, context, cancellationToken});
                            var response = await task;

                            var header = HttpSerializer.FormatHeaders(response);
                            execution.Result = header.ToString();
                            bodyStream = await response.Content.ReadAsStreamAsync();
                        }
                        catch (Exception e)
                        {
                            Logger.LogWarning(e, "An error occurred when executing {method}", executorType.FullName);
                            execution.ExecutionError = e.ToString();
                        }

                        using (bodyStream)
                        {
                            var sessionManager = Services.GetRequiredService<ITaskSessionManager>();
                            await sessionManager.CreateExecution(OrcusTask, taskSession, execution, bodyStream);
                        }
                    }
                }, cancellationToken);
            }
        }
    }
}