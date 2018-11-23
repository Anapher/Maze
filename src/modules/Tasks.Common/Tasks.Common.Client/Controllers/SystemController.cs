using Orcus.Modules.Api;
using Orcus.Modules.Api.Routing;
using System;
using System.Diagnostics;

namespace Tasks.Common.Client.Controllers
{
    [Route("system/power")]
    public class SystemController : OrcusController
    {
        [OrcusGet("shutdown")]
        public IActionResult Shutdown()
        {
            ExecuteShutdown("/s /t 0");
            return Ok();
        }

        [OrcusGet("restart")]
        public IActionResult Restart()
        {
            ExecuteShutdown("/l /t 0");
            return Ok();
        }

        [OrcusGet("logoff")]
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
