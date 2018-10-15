using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orcus.Server.Connection.Tasks;
using Orcus.Server.Data.EfClasses.Tasks;
using Orcus.Server.Data.EfCode;
using Orcus.Server.Library.Tasks;
using Orcus.Server.Service.Tasks.Filter;
using Orcus.Utilities;

namespace Orcus.Server.Service.Tasks
{
    public class OrcusTaskService
    {
        private AggregatedClientFilter _aggregatedClientFilter;

        public OrcusTaskService(OrcusTask orcusTask, IServiceProvider services)
        {
            OrcusTask = orcusTask;
            Services = services;
        }

        public OrcusTask OrcusTask { get; }
        public ILogger Logger { get; set; }
        public IServiceProvider Services { get; }

        public async Task Run(CancellationToken cancellationToken)
        {
            var loggerFactory = Services.GetRequiredService<ILoggerFactory>();

            var filters = new List<FilterFactory>();
            foreach (var filterInfo in OrcusTask.Filters)
            {
                var filterType = typeof(IFilterService<>).MakeGenericType(filterInfo.GetType());
                filters.Add(new FilterFactory(filterType, filterInfo, loggerFactory.CreateLogger<FilterFactory>()));
            }
            filters = filters.OrderBy(x => x.Cost ?? 5).ToList();

            _aggregatedClientFilter = new AggregatedClientFilter(filters, new AudienceFilter(OrcusTask.Audience),
                loggerFactory.CreateLogger<AggregatedClientFilter>());

            using (var triggerScope = Services.CreateScope())
            {
                var tasks = new Dictionary<Task, Type>();

                foreach (var triggerInfo in OrcusTask.Triggers)
                {
                    var serviceType = typeof(ITriggerService<>).MakeGenericType(triggerInfo.GetType());
                    var service = triggerScope.ServiceProvider.GetService(serviceType);
                    if (service == null)
                    {
                        Logger.LogError("The transmission service for type {transmissionInfo} ({resolvedType}) could not be resolved. Skipped.",
                            triggerInfo.GetType(), serviceType);
                        continue;
                    }

                    var triggerContext = new TaskTriggerContext(this, serviceType.Name, _aggregatedClientFilter);
                    var methodInfo = serviceType.GetMethod("InvokeAsync", BindingFlags.Instance);

                    try
                    {
                        var task = (Task) methodInfo.Invoke(service, new object[] {triggerInfo, triggerContext, cancellationToken});
                        tasks.Add(task, serviceType);
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e, "Error occurred when invoking transmission service {transmissionServiceType}", serviceType);
                    }
                }

                while (tasks.Any())
                {
                    var task = await Task.WhenAny(tasks.Keys);

                    if (task.IsFaulted)
                    {
                        var type = tasks[task];
                        Logger.LogError(task.Exception, "An error occurred when awaiting the trigger {trigger}", type);
                    }

                    tasks.Remove(task);
                }
            }
        }

        public async Task TriggerNow()
        {
            var context = new TaskTriggerContext(this, "Manually Triggered", _aggregatedClientFilter);
            var session = await context.CreateSession(SessionKey.Create("ManualTrigger", DateTimeOffset.UtcNow));
            await session.InvokeAll();
        }

        internal async Task Execute(IEnumerable<TargetId> targetIds, TaskSession taskSession, CancellationToken cancellationToken)
        {
            using (var executionScope = Services.CreateScope())
            {
                var attenderScopes = new ConcurrentDictionary<TargetId, IServiceScope>();
                var attenders = await TaskCombinators.ThrottledFilterItems(targetIds, async (targetId, token) =>
                {
                    if (targetId.IsServer)
                        return OrcusTask.Audience.IncludesServer;

                    var scope = executionScope.ServiceProvider.CreateScope();
                    if (await _aggregatedClientFilter.IsClientIncluded(targetId.ClientId, scope.ServiceProvider))
                    {
                        if (!attenderScopes.TryAdd(targetId, scope))
                            scope.Dispose();

                        return true;
                    }

                    scope.Dispose();
                    return false;
                }, cancellationToken);
                
                var context = new DefaultTaskExecutionContext(taskSession, OrcusTask);
                foreach (var orcusTaskCommand in OrcusTask.Commands)
                {
                    var executorType = typeof(ITaskExecutor<>).MakeGenericType(orcusTaskCommand.GetType());

                    var localService = executionScope.ServiceProvider.GetService(executorType);
                    if (localService != null)
                    {
                        var executionMethod = executorType.GetMethod("InvokeAsync", BindingFlags.Instance);
                        var commandName = orcusTaskCommand.GetType().Name.Replace("CommandInfo", null);

                        await TaskCombinators.ThrottledAsync(attenders, async (id, token) =>
                        {
                            if (!attenderScopes.TryGetValue(id, out var scope))
                                scope = executionScope.ServiceProvider.CreateScope();

                            using (scope)
                            {
                                var service = scope.ServiceProvider.GetRequiredService(executorType);

                                var execution = new TaskExecution
                                {
                                    TaskSessionId = taskSession.TaskSessionId, TargetId = id.IsServer ? (int?) null : id.ClientId,
                                    Timestamp = DateTimeOffset.UtcNow, CommandName = commandName
                                };
                                try
                                {
                                    var task = (Task<HttpResponseMessage>) executionMethod.Invoke(service,
                                        new object[] {orcusTaskCommand, id, context, cancellationToken});
                                    var response = await task;
                                    //todo serialize http
                                }
                                catch (Exception e)
                                {
                                    Logger.LogWarning(e, "An error occurred when executing {method}", executorType.FullName);
                                    execution.ExecutionError = e.ToString();
                                }

                                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                                dbContext.Add(execution);
                                await dbContext.SaveChangesAsync();
                            }
                        }, cancellationToken);
                    }
                }
            }
        }
    }
}