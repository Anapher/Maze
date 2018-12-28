using System.Collections.Concurrent;
using Maze.Server.Library.Services;

namespace Maze.Server.Service.Connection
{
    public class ConnectionManager : IConnectionManager
    {
        public ConcurrentDictionary<int, IClientConnection> ClientConnections { get; } = new ConcurrentDictionary<int, IClientConnection>();
        public ConcurrentDictionary<int, IAdministrationConnection> AdministrationConnections { get; } = new ConcurrentDictionary<int, IAdministrationConnection>();
    }
}