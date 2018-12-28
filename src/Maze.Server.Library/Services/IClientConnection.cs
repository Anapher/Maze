using Maze.Server.Library.Clients;
using Maze.Sockets;

namespace Maze.Server.Library.Services
{
    /// <summary>
    ///     A client connection that is currently active.
    /// </summary>
    public interface IClientConnection : IRestClient
    {
        /// <summary>
        ///     The id of the client
        /// </summary>
        int ClientId { get; }

        /// <summary>
        ///     The connection server.
        /// </summary>
        MazeServer MazeServer { get; }
    }
}