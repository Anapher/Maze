using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tasks.Infrastructure.Core.Filter;
using Tasks.Infrastructure.Server.Library;

namespace Tasks.Infrastructure.Server.Filter
{
    public class CustomFilterFactory : IClientFilter
    {
        private readonly ILogger<CustomFilterFactory> _logger;
        private bool _isSkipped;

        public CustomFilterFactory(Type filterType, FilterInfo filterInfo, ILogger<CustomFilterFactory> logger)
        {
            FilterType = filterType;
            FilterInfo = filterInfo;
            _logger = logger;

            var costAttribute = filterType.GetCustomAttribute<FilterPerformanceCostAttribute>();
            Cost = costAttribute?.Value;
        }

        public Type FilterType { get; }
        public int? Cost { get; }
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

        public override string ToString()
        {
            return $"Custom - {FilterType.FullName}";
        }
    }
}