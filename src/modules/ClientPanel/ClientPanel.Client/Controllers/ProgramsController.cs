using System.Diagnostics;
using Orcus.Modules.Api;
using Orcus.Modules.Api.Routing;

namespace ClientPanel.Client.Controllers
{
    [Route("programs")]
    public class ProgramsController : OrcusController
    {
        [OrcusGet("taskManager")]
        public IActionResult StartTaskManager()
        {
            Process.Start("taskmgr.exe");
            return Ok();
        }

        [OrcusGet("regEdit")]
        public IActionResult StartRegEdit()
        {
            Process.Start("regedit.exe");
            return Ok();
        }

        [OrcusGet("deviceManager")]
        public IActionResult StartDeviceManager()
        {
            Process.Start("Devmgmt.msc");
            return Ok();
        }

        [OrcusGet("eventLog")]
        public IActionResult StartEventLog()
        {
            Process.Start("eventvwr.msc");
            return Ok();
        }

        [OrcusGet("controlPanel")]
        public IActionResult StartControlPanel()
        {
            Process.Start("control");
            return Ok();
        }

        [OrcusGet("services")]
        public IActionResult StartServices()
        {
            Process.Start("services.msc");
            return Ok();
        }

        [OrcusGet("computerManagement")]
        public IActionResult StartComputerManagement()
        {
            Process.Start("Compmgmt.msc");
            return Ok();
        }
    }
}