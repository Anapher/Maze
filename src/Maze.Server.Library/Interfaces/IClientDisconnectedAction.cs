using Maze.Modules.Api;

namespace Maze.Server.Library.Interfaces
{
    /// <summary>
    ///     An action that will be invoked once a client disconnected from the server. The context is the
    ///     id of the client that disconnected.
    /// </summary>
    public interface IClientDisconnectedAction : IActionInterface<int>
    {
    }
}