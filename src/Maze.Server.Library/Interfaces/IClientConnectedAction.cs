using Maze.Modules.Api;
using Maze.Server.Library.Services;

namespace Maze.Server.Library.Interfaces
{
    /// <summary>
    ///     An action that will be invoked once a client connects to the server.
    /// </summary>
    public interface IClientConnectedAction : IActionInterface<IClientConnection>
    {
    }
}