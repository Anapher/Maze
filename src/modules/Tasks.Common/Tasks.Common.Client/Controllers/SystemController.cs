using Maze.Modules.Api;
using Maze.Modules.Api.Routing;
using System;
using System.Diagnostics;

namespace Tasks.Common.Client.Controllers
{
    [Route("system/power")]
    public class SystemController : MazeController
    {
        [MazeGet("shutdown")]
        public IActionResult Shutdown()
        {
            ExecuteShutdown("/s /t 0");
            return Ok();
        }

        [MazeGet("restart")]
        public IActionResult Restart()
        {
            ExecuteShutdown("/l /t 0");
            return Ok();
        }

        [MazeGet("logoff")]
        public IActionResult LogOff()
        {
            ExecuteShutdown("/r /t 0");
            return Ok();
        }

        private void ExecuteShutdown(string arguments)
        {
            var psi = new ProcessStartInfo("shutdown.exe", arguments)
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };

            var process = Process.Start(psi);
            if (process == null)
                throw new InvalidOperationException("Unable to start shutdown.exe");
        }
    }
}
