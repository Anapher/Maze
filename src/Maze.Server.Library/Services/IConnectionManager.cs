using System.Collections.Concurrent;

namespace Maze.Server.Library.Services
{
    public interface IConnectionManager
    {
        ConcurrentDictionary<int, IClientConnection> ClientConnections { get; }
        ConcurrentDictionary<int, IAdministrationConnection> AdministrationConnections { get; }
    }
}