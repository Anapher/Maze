using Orcus.Clients;
using Orcus.Server.Connection.Modules;

namespace Orcus.Core.Connection
{
    public class ServerConnection
    {
        public ServerConnection(IRestClient restClient, PackagesLock packagesLock)
        {
            RestClient = restClient;
            PackagesLock = packagesLock;
        }

        public IRestClient RestClient { get; }
        public PackagesLock PackagesLock { get; }
    }
}