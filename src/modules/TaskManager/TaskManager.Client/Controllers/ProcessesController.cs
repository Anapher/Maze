using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using Maze.Modules.Api;
using Maze.Modules.Api.Parameters;
using Maze.Modules.Api.Routing;
using TaskManager.Client.Channels;
using TaskManager.Client.Utilities;
using TaskManager.Shared.Dtos;

namespace TaskManager.Client.Controllers
{
    [Route("processes/{processId}")]
    public class ProcessesController : MazeController
    {
        [MazeGet("kill")]
        public async Task<IActionResult> KillProcess(int processId)
        {
            if (!GetProcess(processId, out var process, out var errorResult))
                return errorResult;

            using (process)
                await process.KillGracefully();

            return Ok();
        }

        [MazeGet("killTree")]
        public async Task<IActionResult> KillProcessTree(int processId)
        {
            if (!await ProcessExtensions.KillProcessTree(processId))
                return NotFound();

            return Ok();
        }

        [MazeGet("suspend")]
        public IActionResult Suspend(int processId)
        {
            if (!GetProcess(processId, out var process, out var errorResult))
                return errorResult;

            using (process)
                process.Suspend();

            return Ok();
        }

        [MazeGet("resume")]
        public IActionResult Resume(int processId)
        {
            if (!GetProcess(processId, out var process, out var errorResult))
                return errorResult;

            using (process)
                process.Resume();

            return Ok();
        }

        [MazeGet("setPriority")]
        public IActionResult SetPriority(int processId, [FromQuery] ProcessPriorityClass priority)
        {
            if (!GetProcess(processId, out var process, out var errorResult))
                return errorResult;

            using (process)
                process.PriorityClass = priority;

            return Ok();
        }

        [MazeGet("connections")]
        public IActionResult GetConnections(int processId)
        {
            return Ok(Connections.GetConnections(processId).ToList());
        }

        [MazeGet("properties")]
        public IActionResult GetProperties(int processId)
        {
            if (!GetProcess(processId, out var process, out var errorResult))
                return errorResult;

            var dto = new ProcessPropertiesDto();
            using (process)
            {
                using (var searcher = new ManagementObjectSearcher("root\\CIMV2", $"SELECT * FROM Win32_Process WHERE ProcessId = {processId}"))
                using (var results = searcher.Get())
                {
                    var wmiProcess = results.Cast<ManagementObject>().SingleOrDefault();
                    if (wmiProcess == null)
                        return NotFound();

                    dto.ApplyProperties(process, wmiProcess);

                    if (wmiProcess.TryGetProperty("ExecutablePath", out string executablePath))
                        try
                        {
                            dto.Icon = FileUtilities.GetFileIcon(executablePath, 32);
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                }

                try
                {
                    dto.IsWow64Process = ProcessExtensions.Is64BitProcess(process.Handle);
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            return Ok(dto);
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