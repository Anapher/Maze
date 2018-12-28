using System.Diagnostics;
using Maze.Modules.Api;
using Maze.Modules.Api.Routing;

namespace ClientPanel.Client.Controllers
{
    [Route("programs")]
    public class ProgramsController : MazeController
    {
        [MazeGet("taskManager")]
        public IActionResult StartTaskManager()
        {
            Process.Start("taskmgr.exe");
            return Ok();
        }

        [MazeGet("regEdit")]
        public IActionResult StartRegEdit()
        {
            Process.Start("regedit.exe");
            return Ok();
        }

        [MazeGet("deviceManager")]
        public IActionResult StartDeviceManager()
        {
            Process.Start("Devmgmt.msc");
            return Ok();
        }

        [MazeGet("eventLog")]
        public IActionResult StartEventLog()
        {
            Process.Start("eventvwr.msc");
            return Ok();
        }

        [MazeGet("controlPanel")]
        public IActionResult StartControlPanel()
        {
            Process.Start("control");
            return Ok();
        }

        [MazeGet("services")]
        public IActionResult StartServices()
        {
            Process.Start("services.msc");
            return Ok();
        }

        [MazeGet("computerManagement")]
        public IActionResult StartComputerManagement()
        {
            Process.Start("Compmgmt.msc");
            return Ok();
        }
    }
}