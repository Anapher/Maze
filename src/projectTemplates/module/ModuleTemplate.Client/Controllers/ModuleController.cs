using Maze.Modules.Api;
using Maze.Modules.Api.Routing;

namespace ModuleTemplate.Client.Controllers
{
    public class ModuleNamePlaceholderController : MazeController
    {
        [MazeGet]
        public IActionResult TestAction()
        {
            return Ok("Hello from ModuleNamePlaceholder!");
        }
    }
}