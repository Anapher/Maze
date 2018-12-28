using System.Threading.Tasks;
using Maze.Modules.Api;
using Maze.Modules.Api.Request;
using Maze.Modules.Api.Response;

namespace Maze.Service.Commander
{
    /// <summary>
    ///     Take an <see cref="MazeRequest" /> and execute it, responding with an <see cref="MazeResponse" />
    /// </summary>
    public interface IMazeRequestExecuter
    {
        /// <summary>
        ///     Execute the given <see cref="MazeRequest" />
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="channelServer">The server that manages the active channel</param>
        /// <returns>Return the result of the request</returns>
        Task Execute(MazeContext context, IChannelServer channelServer);
    }
}