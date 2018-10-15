using System;
using System.Linq;
using Orcus.Server.Connection.Commanding;
using Orcus.Server.Connection.Tasks.Audience;

namespace Orcus.Server.Service.Tasks.Filter
{
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
}