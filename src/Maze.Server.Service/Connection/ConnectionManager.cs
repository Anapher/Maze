using System.Collections.Concurrent;
using Orcus.Server.Library.Services;

namespace Orcus.Server.Service.Connection
{
    public class ConnectionManager : IConnectionManager
    {
        public ConcurrentDictionary<int, IClientConnection> ClientConnections { get; } = new ConcurrentDictionary<int, IClientConnection>();
        public ConcurrentDictionary<int, IAdministrationConnection> AdministrationConnections { get; } = new ConcurrentDictionary<int, IAdministrationConnection>();
    }
}