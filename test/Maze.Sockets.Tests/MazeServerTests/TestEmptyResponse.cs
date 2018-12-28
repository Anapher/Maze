using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Maze.Modules.Api.Response;

namespace Maze.Sockets.Tests.MazeServerTests
{
    public class TestEmptyResponse : MazeServerTestBase
    {
        protected override Task WriteResponse(MazeResponse response)
        {
            response.StatusCode = StatusCodes.Status404NotFound;
            return Task.CompletedTask;
        }
    }
}