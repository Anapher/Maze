using Maze.Server.Connection.Modules;

namespace Maze.Client.Library.Services
{
    public interface IServerConnection
    {
        IMazeRestClient RestClient { get; }
        PackagesLock PackagesLock { get; }
    }
}
