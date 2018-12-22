using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orcus.Server.Connection.Extensions;
using Orcus.Utilities;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Management.Data;
using Tasks.Infrastructure.Server.Filter;
using Tasks.Infrastructure.Server.Library;

namespace Tasks.Infrastructure.Server.Core
{
    public class OrcusTaskService : INotifyPropertyChanged
    {
        private AggregatedClientFilter _aggregatedClientFilter;
        private DateTimeOffset _nextExecution;

        public OrcusTaskService(OrcusTask orcusTask, IServiceProvider services)
        {
            OrcusTask = orcusTask;
            Services = services;
            Logger = services.GetRequiredService<ILogger<OrcusTaskService>>();
        }

        public OrcusTask OrcusTask { get; }
        public ILogger Logger { get; }
        public IServiceProvider Services { get; }

        public DateTimeOffset NextExecution
        {
            get => _nextExecution;
            set
            {
                _nextExecution = value;
                OnPropertyChanged();
            }
        }

        private void InitializeFilter()
        {
            if (_aggregatedClientFilter != null)
                return;

            var loggerFactory = Services.GetRequiredService<ILoggerFactory>();

            var filters = new AggregatedClientFilter(loggerFactory.CreateLogger<AggregatedClientFilter>());

            filters.Add(new AudienceFilter(OrcusTask.Audience));
            foreach (var filterInfo in OrcusTask.Filters)
            {
                var filterType = typeof(IFilterService<>).MakeGenericType(filterInfo.GetType());
                filters.Add(new CustomFilterFactory(filterType, filterInfo, loggerFactory.CreateLogger<CustomFilterFactory>()));
            }

            _aggregatedClientFilter = filters;
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            InitializeFilter();

            using (var triggerScope = Services.CreateScope())
            {
                var tasks = new Dictionary<Task, Type>();

                foreach (var triggerInfo in OrcusTask.Triggers)
                {
                    var serviceType = typeof(ITriggerService<>).MakeGenericType(triggerInfo.GetType());
                    var service = triggerScope.ServiceProvider.GetService(serviceType);
                    if (service == null)
                    {
                        Logger.LogWarning("The trigger service for type {triggerInfo} ({resolvedType}) could not be resolved. Skipped.",
                            triggerInfo.GetType(), serviceType);
                        continue;
                    }

                    var triggerContext = new TaskTriggerContext(this, service.GetType().Name.TrimEnd("TriggerService", StringComparison.Ordinal),
                        _aggregatedClientFilter);
                    var methodInfo = serviceType.GetMethod("InvokeAsync", BindingFlags.Instance | BindingFlags.Public);

                    try
                    {
                        var task = Task.Run(async () =>
                            await (Task) methodInfo.Invoke(service, new object[] {triggerInfo, triggerContext, cancellationToken}));
                        tasks.Add(task, serviceType);
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e, "Error occurred when invoking trigger service {triggerServiceType}", serviceType);
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
            InitializeFilter();

            var context = new TaskTriggerContext(this, "Manually Triggered", _aggregatedClientFilter);
            var session = await context.CreateSession(SessionKey.Create("ManualTrigger", DateTimeOffset.UtcNow));
            await session.Invoke();
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

                var executor = new TaskExecutor(OrcusTask, taskSession, Services);
                await executor.Execute(attenders, attenderScopes, cancellationToken);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}