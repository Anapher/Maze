using System.Collections.Concurrent;

namespace Orcus.Server.Service.Connection
{
    public class ConnectionManager : IConnectionManager
    {
        public ConcurrentDictionary<int, ClientConnection> ClientConnections { get; }
        public ConcurrentDictionary<int, AdministrationConnection> AdministrationConnections { get; }
    }
}