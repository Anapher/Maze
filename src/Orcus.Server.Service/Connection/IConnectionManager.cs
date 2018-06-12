using System.Collections.Concurrent;

namespace Orcus.Server.Service.Connection
{
    public interface IConnectionManager
    {
        ConcurrentDictionary<int, ClientConnection> ClientConnections { get; }
        ConcurrentDictionary<int, AdministrationConnection> AdministrationConnections { get; }
    }
}