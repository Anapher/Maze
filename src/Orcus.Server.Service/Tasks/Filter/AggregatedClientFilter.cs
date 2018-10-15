using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Orcus.Server.Service.Tasks.Filter
{
    public class AggregatedClientFilter
    {
        private readonly IReadOnlyList<FilterFactory> _filters;
        private readonly AudienceFilter _audienceFilter;
        private readonly ILogger _logger;
        private readonly Dictionary<int, bool> _cachedResults;

        public AggregatedClientFilter(IReadOnlyList<FilterFactory> filters, AudienceFilter audienceFilter, ILogger<AggregatedClientFilter> logger)
        {
            _filters = filters;
            _audienceFilter = audienceFilter;
            _logger = logger;
            _cachedResults = new Dictionary<int, bool>();
        }

        public async Task<bool> IsClientIncluded(int clientId, IServiceProvider serviceProvider)
        {
            if (!_cachedResults.TryGetValue(clientId, out var approveStatus))
                _cachedResults[clientId] = approveStatus = await InternalIsClientIncluded(clientId, serviceProvider);

            return approveStatus;
        }

        private async Task<bool> InternalIsClientIncluded(int clientId, IServiceProvider serviceProvider)
        {
            if (!_audienceFilter.IsIncluded(clientId))
                return false;

            if (_filters.Any())
            {
                //scope so things like DbContext are shared and objects are only queried once
                foreach (var filter in _filters)
                {
                    try
                    {
                        if (!await filter.Invoke(clientId, serviceProvider))
                            return false;
                    }
                    catch (Exception e)
                    {
                        _logger.LogWarning(e,
                            "Filter {filterType} failed to invoke on client {clientId} with payload {@payload}. The filter will be ignored.",
                            filter.FilterType.FullName, clientId, filter.FilterInfo);
                    }
                }

            }

            return true;
        }
    }
}