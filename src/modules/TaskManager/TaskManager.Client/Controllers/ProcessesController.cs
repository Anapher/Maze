using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Orcus.Modules.Api;
using Orcus.Modules.Api.Parameters;
using Orcus.Modules.Api.Routing;
using TaskManager.Client.Utilities;

namespace TaskManager.Client.Controllers
{
    [Route("processes/{processId}")]
    public class ProcessesController : OrcusController
    {
        [OrcusGet("kill")]
        public async Task<IActionResult> KillProcess(int processId)
        {
            if (!GetProcess(processId, out var process, out var errorResult))
                return errorResult;

            using (process)
            {
                await process.KillGracefully();
            }

            return Ok();
        }

        [OrcusGet("killTree")]
        public async Task<IActionResult> KillProcessTree(int processId)
        {
            if (!await ProcessExtensions.KillProcessTree(processId))
                return NotFound();

            return Ok();
        }

        [OrcusGet("suspend")]
        public IActionResult Suspend(int processId)
        {
            if (!GetProcess(processId, out var process, out var errorResult))
                return errorResult;

            process.Suspend();
            return Ok();
        }

        [OrcusGet("resume")]
        public IActionResult Resume(int processId)
        {
            if (!GetProcess(processId, out var process, out var errorResult))
                return errorResult;

            process.Resume();
            return Ok();
        }

        [OrcusGet("setPriority")]
        public IActionResult SetPriority(int processId, [FromQuery] ProcessPriorityClass priority)
        {
            if (!GetProcess(processId, out var process, out var errorResult))
                return errorResult;

            process.PriorityClass = priority;
            return Ok();
        }

        private bool GetProcess(int processId, out Process process, out IActionResult errorResult)
        {
            try
            {
                process = Process.GetProcessById(processId);
            }
            catch (Exception)
            {
                errorResult = NotFound();
                process = null;
                return false;
            }

            errorResult = null;
            return true;
        }
    }
}