using Maze.Modules.Api;
using Maze.Modules.Api.Routing;

namespace TestModule.Server
{
    public class TestController : MazeController
    {
        [MazeGet]
        public IActionResult Test() => Ok("Hello World");

        [MazeGet("core/wtf/{id}")]
        public IActionResult HelloWorld(int id)
        {
            return Ok(new TestData {Data = new[] {"Hey", "wtf", "this shit is fucking shit"}, Id = id });
        }
    }

    public class TestData
    {
        public int Id { get; set; }
        public string[] Data { get; set; }
    }
}