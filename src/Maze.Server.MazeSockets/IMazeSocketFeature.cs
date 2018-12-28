using System.Threading.Tasks;
using Maze.Sockets;

namespace Maze.Server.MazeSockets
{
    /// <summary>
    ///     Activator of an <see cref="MazeSocket" />
    /// </summary>
    public interface IMazeSocketFeature
    {
        /// <summary>
        ///     Upgrade the connection and create the <see cref="MazeSocket" />
        /// </summary>
        /// <returns>Return the created <see cref="MazeSocket" /></returns>
        Task<MazeSocket> AcceptAsync();
    }
}