using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Tasks.Infrastructure.Server.Filter
{
    public class AggregatedClientFilter : List<IClientFilter>
    {
        private readonly ILogger _logger;
        private readonly Dictionary<int, bool> _cachedResults;

        public AggregatedClientFilter(ILogger<AggregatedClientFilter> logger)
        {
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
            foreach (var filter in this.OrderBy(x => x.Cost))
            {
                try
                {
                    if (!await filter.Invoke(clientId, serviceProvider))
                        return false;
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e,
                        "Filter {filterType} failed to invoke on client {clientId}. The filter will be ignored.",
                        filter.ToString(), clientId);
                }
            }

            return true;
        }
    }
}