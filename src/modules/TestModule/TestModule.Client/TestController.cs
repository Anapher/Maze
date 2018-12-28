using System;
using Maze.Modules.Api;
using Maze.Modules.Api.Routing;

namespace TestModule.Client
{
    public class TestController : MazeController
    {
        [MazeGet]
        public IActionResult Test() => Ok("Processor Count on this PC: " + Environment.ProcessorCount);
    }
}