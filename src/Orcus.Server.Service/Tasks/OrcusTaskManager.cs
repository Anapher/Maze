using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orcus.Server.BusinessLogic.Tasks;
using Orcus.Server.Connection.Commanding;
using Orcus.Server.Connection.Tasks;
using Orcus.Server.Connection.Tasks.Audience;
using Orcus.Server.Connection.Tasks.Filter;
using Orcus.Server.Data.EfClasses.Tasks;
using Orcus.Server.Data.EfCode;
using Orcus.Server.Library.Tasks;

namespace Orcus.Server.Service.Tasks
{
    public class OrcusTaskManager
    {
        
    }

    public class AggregatedClientFilter
    {
        private readonly IReadOnlyList<FilterFactory> _filters;
        private readonly AudienceFilter _audienceFilter;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private readonly Dictionary<int, bool> _cachedResults;

        public AggregatedClientFilter(IReadOnlyList<FilterFactory> filters, AudienceFilter audienceFilter, IServiceProvider serviceProvider,
            ILogger<AggregatedClientFilter> logger)
        {
            _filters = filters;
            _audienceFilter = audienceFilter;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _cachedResults = new Dictionary<int, bool>();
        }

        public async Task<bool> IsClientIncluded(int clientId)
        {
            if (!_cachedResults.TryGetValue(clientId, out var approveStatus))
               _cachedResults[clientId] = approveStatus = await InternalIsClientIncluded(clientId);

            return approveStatus;
        }

        private async Task<bool> InternalIsClientIncluded(int clientId)
        {
            if (!_audienceFilter.IsIncluded(clientId))
                return false;

            if (_filters.Any())
            {
                //scope so things like DbContext are shared and objects are only queried once
                using (var scope = _serviceProvider.CreateScope())
                {
                    foreach (var filter in _filters)
                    {
                        try
                        {
                            if (!await filter.Invoke(clientId, scope.ServiceProvider))
                                return false;
                        }
                        catch (Exception e)
                        {
                            _logger.LogWarning(e, "Filter {filterType} failed to invoke on client {clientId} with payload {@payload}. The filter will be ignored.",
                                filter.FilterType.FullName, clientId, filter.FilterInfo);
                        }
                    }
                }
            }

            return true;
        }
    }

    public class AudienceFilter
    {
        private readonly AudienceCollection _audienceCollection;

        public AudienceFilter(AudienceCollection audienceCollection)
        {
            _audienceCollection = audienceCollection;
        }

        public bool IsIncluded(int clientId)
        {
            if (_audienceCollection.IsAll)
                return true;

            if (_audienceCollection.Any(x => x.Type == CommandTargetType.Client && clientId >= x.From && clientId <= x.To))
                return true;

            throw new NotImplementedException();
        }

        public bool IsServerIncluded() => _audienceCollection.IncludesServer;
    }

    public class TaskSessionService : TaskTransmissionSession
    {
        private readonly OrcusTaskService _taskService;

        public TaskSessionService(OrcusTaskService taskService, TaskSession taskSession)
        {
            _taskService = taskService;
            Info = taskSession;
        }

        public override TaskSession Info { get; }

        public override Task InvokeAll()
        {
            throw new NotImplementedException();
        }

        public override Task InvokeClient(int clientId)
        {
            throw new NotImplementedException();
        }

        public override Task InvokeServer()
        {
            throw new NotImplementedException();
        }
    }

    public class TaskTransmissionContext : TransmissionContext
    {
        private readonly OrcusTaskService _taskService;
        private readonly string _sourceTrigger;
        private readonly AggregatedClientFilter _aggregatedClientFilter;

        public TaskTransmissionContext(OrcusTaskService taskService, string sourceTrigger, AggregatedClientFilter aggregatedClientFilter)
        {
            _taskService = taskService;
            _sourceTrigger = sourceTrigger;
            _aggregatedClientFilter = aggregatedClientFilter;
        }

        public override async Task<TaskTransmissionSession> GetSession(string name)
        {
            TaskSession taskSession;
            using (var scope = _taskService.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var action = _taskService.Services.GetRequiredService<IGetOrCreateTaskSessionAction>();
                taskSession = await action.BizActionAsync((_taskService.OrcusTask.Id, name, _sourceTrigger));
                if (action.HasErrors)
                    throw new InvalidOperationException(action.Errors.First().ErrorMessage);

                await context.SaveChangesAsync();
            }

            return new TaskSessionService(_taskService, taskSession);
        }

        public override Task<bool> IsClientIncluded(int clientId)
        {
            return _aggregatedClientFilter.IsClientIncluded(clientId);
        }

        public override bool IsServerIncluded()
        {
            return _taskService.OrcusTask.Audience.IncludesServer;
        }
    }

    public class FilterFactory
    {
        private readonly ILogger<FilterFactory> _logger;
        private bool _isSkipped;

        public FilterFactory(Type filterType, FilterInfo filterInfo, ILogger<FilterFactory> logger)
        {
            FilterType = filterType;
            FilterInfo = filterInfo;
            _logger = logger;
        }

        public Type FilterType { get; }
        public FilterInfo FilterInfo { get; }

        public Task<bool> Invoke(int clientId, IServiceProvider serviceProvider)
        {
            if (_isSkipped)
                return Task.FromResult(true);

            var filter = serviceProvider.GetService(FilterType);
            if (filter == null)
            {
                _logger.LogError("The filter service for type {filterType} ({resolvedType}) could not be resolved. Skipped.", FilterInfo.GetType(),
                    FilterType);

                _isSkipped = true;
                return Task.FromResult(true); //just skip the filter
            }

            var methodInfo = filter.GetType().GetMethod("IncludeClient", BindingFlags.Instance);
            var task = (Task<bool>) methodInfo.Invoke(filter, new object[] {FilterInfo, clientId});
            return task;
        }
    }

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

        public Task Run()
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

            foreach (var transmissionInfo in OrcusTask.Transmission)
            {
                var serviceType = typeof(ITransmissionService<>).MakeGenericType(transmissionInfo.GetType());
                var service = Services.GetRequiredService(serviceType);
                if (service == null)
                {
                    Logger.LogError("The transmission service for type {transmissionInfo} ({resolvedType}) could not be resolved. Skipped.",
                        transmissionInfo.GetType(), serviceType);
                    continue;
                }

                var transmissionContext = new TaskTransmissionContext(this, serviceType.Name, _aggregatedClientFilter);
                var methodInfo = serviceType.GetMethod("InvokeAsync", BindingFlags.Instance);

                try
                {
                    methodInfo.Invoke(service, new object[] {transmissionInfo, transmissionContext, _cancellationTokenSource.Token});
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "Error occurred when invoking transmission service {transmissionServiceType}", serviceType);
                }
            }

            return Task.CompletedTask;
        }

        public async Task TriggerNow()
        {
            var context = new TaskTransmissionContext(this, "Manual Trigger", _aggregatedClientFilter);
            var session = await context.GetSession($"Manual session {DateTimeOffset.UtcNow:G}");
            await session.InvokeAll();
        }
    }
}
