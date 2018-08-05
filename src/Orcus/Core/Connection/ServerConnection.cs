using Orcus.Server.Connection.Modules;

namespace Orcus.Core.Connection
{
    public class ServerConnection
    {
        public ServerConnection(IOrcusRestClient restClient, PackagesLock packagesLock)
        {
            RestClient = restClient;
            PackagesLock = packagesLock;
        }

        public IOrcusRestClient RestClient { get; }
        public PackagesLock PackagesLock { get; }
    }
}