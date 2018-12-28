using Maze.Sockets;

namespace Maze.Server.Library.Services
{
    /// <summary>
    ///     An administration connection that is currently active.
    /// </summary>
    public interface IAdministrationConnection
    {
        /// <summary>
        ///     The account id of the user that is administrating.
        /// </summary>
        int AccountId { get; }

        /// <summary>
        ///     The connection server.
        /// </summary>
        MazeServer MazeServer { get; }
    }
}