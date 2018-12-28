using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Maze.Modules.Api.Request;
using Maze.Server.Connection.Commanding;

namespace Maze.Server.Service
{
    public interface ICommandDistributer
    {
        Task Execute(MazeRequest request, CommandTargetCollection targets, CommandExecutionPolicy executionPolicy);
        Task<HttpResponseMessage> Execute(HttpRequestMessage request, int clientId, int accountId, CancellationToken cancellationToken);
    }
}