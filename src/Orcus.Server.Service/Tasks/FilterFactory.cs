using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orcus.Server.Connection.Tasks.Filter;

namespace Orcus.Server.Service.Tasks
{
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
}