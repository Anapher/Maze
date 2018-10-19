using System;
using System.Linq;
using System.Threading.Tasks;
using Orcus.Server.Connection.Commanding;
using Tasks.Infrastructure.Core.Audience;

namespace Tasks.Infrastructure.Server.Filter
{
    public class AudienceFilter : IClientFilter
    {
        private readonly AudienceCollection _audienceCollection;

        public AudienceFilter(AudienceCollection audienceCollection)
        {
            _audienceCollection = audienceCollection;
        }
        
        public bool IsServerIncluded() => _audienceCollection.IncludesServer;
        public int? Cost { get; } = 0;

        public Task<bool> Invoke(int clientId, IServiceProvider serviceProvider)
        {
            if (_audienceCollection.IsAll)
                return Task.FromResult(true);

            if (_audienceCollection.Any(x => x.Type == CommandTargetType.Client && clientId >= x.From && clientId <= x.To))
                return Task.FromResult(true);

            //check group
            throw new NotImplementedException();
        }
    }
}