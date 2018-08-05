using System.Collections.Concurrent;

namespace Orcus.Server.Service.Connection
{
    public class ConnectionManager : IConnectionManager
    {
        public ConcurrentDictionary<int, ClientConnection> ClientConnections { get; } = new ConcurrentDictionary<int, ClientConnection>();
        public ConcurrentDictionary<int, AdministrationConnection> AdministrationConnections { get; } = new ConcurrentDictionary<int, AdministrationConnection>();
    }
}