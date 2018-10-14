using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orcus.Server.Connection.Tasks;
using Orcus.Server.Connection.Tasks.Commands;
using Orcus.Server.Data.EfClasses.Tasks;
using Orcus.Server.Library.Tasks;
using Orcus.Utilities;

namespace Orcus.Server.Service.Tasks
{
    public class OrcusTaskService
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private AggregatedClientFilter _aggregatedClientFilter;

        public OrcusTaskService(OrcusTask orcusTask, IServiceProvider services)
        {
            OrcusTask = orcusTask;
            Services = services;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public OrcusTask OrcusTask { get; }
        public ILogger Logger { get; set; }
        public IServiceProvider Services { get; }

        public async Task Run()
        {
            var loggerFactory = Services.GetRequiredService<ILoggerFactory>();

            var filters = new List<FilterFactory>();
            foreach (var filterInfo in OrcusTask.Filters)
            {
                var filterType = typeof(IFilterService<>).MakeGenericType(filterInfo.GetType());
                filters.Add(new FilterFactory(filterType, filterInfo, loggerFactory.CreateLogger<FilterFactory>()));
            }

            _aggregatedClientFilter = new AggregatedClientFilter(filters, new AudienceFilter(OrcusTask.Audience), Services,
                loggerFactory.CreateLogger<AggregatedClientFilter>());

            using (var transmissionScope = Services.CreateScope())
            {
                var tasks = new List<Task>();

                foreach (var transmissionInfo in OrcusTask.Transmission)
                {
                    var serviceType = typeof(ITriggerService<>).MakeGenericType(transmissionInfo.GetType());
                    var service = transmissionScope.ServiceProvider.GetService(serviceType);
                    if (service == null)
                    {
                        Logger.LogError("The transmission service for type {transmissionInfo} ({resolvedType}) could not be resolved. Skipped.",
                            transmissionInfo.GetType(), serviceType);
                        continue;
                    }

                    var transmissionContext = new TaskTriggerContext(this, serviceType.Name, _aggregatedClientFilter);
                    var methodInfo = serviceType.GetMethod("InvokeAsync", BindingFlags.Instance);

                    try
                    {
                        var task = (Task) methodInfo.Invoke(service, new object[] {transmissionInfo, transmissionContext, _cancellationTokenSource.Token});
                        tasks.Add(task);
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e, "Error occurred when invoking transmission service {transmissionServiceType}", serviceType);
                    }
                }

                while (true)
                {
                    await Task.WhenAll(tasks);
                }
            }
        }

        public async Task TriggerNow()
        {
            var context = new TaskTriggerContext(this, "Manual Trigger", _aggregatedClientFilter);
            var session = await context.GetSession($"Manual session {DateTimeOffset.UtcNow:G}");
            await session.InvokeAll();
        }

        public async Task Execute(IEnumerable<TargetId> targetIds, TaskSession taskSession, CancellationToken cancellationToken)
        {
            var attenders = await TaskCombinators.ThrottledApproveItems(targetIds, async (targetId, token) =>
            {
                if (targetId.IsServer)
                    return OrcusTask.Audience.IncludesServer;

                return await _aggregatedClientFilter.IsClientIncluded(targetId.ClientId);
            }, cancellationToken);

            var localExecutors = new List<(CommandInfo, Type)>();
            var remoteExecutors = new List<CommandInfo>();

            foreach (var orcusTaskCommand in OrcusTask.Commands)
            {
                var executorType = typeof(ITaskExecutor<>).MakeGenericType(orcusTaskCommand.GetType());
                var localService = Services.GetService(executorType);
                if (localService == null)
                    remoteExecutors.Add(orcusTaskCommand);
                else
                    localExecutors.Add((orcusTaskCommand, executorType));
            }


        }
    }
}