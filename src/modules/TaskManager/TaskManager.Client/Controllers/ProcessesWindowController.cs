using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Orcus.Modules.Api;
using Orcus.Modules.Api.Routing;
using TaskManager.Client.Native;

namespace TaskManager.Client.Controllers
{
    [Route("processes/{processId}/window")]
    public class ProcessesWindowController : OrcusController
    {
        [OrcusGet("bringToFront")]
        public IActionResult BringToFront(int processId)
        {
            if (!GetWindowHandle(processId, out var windowHandle, out var errorResult))
                return errorResult;

            if (NativeMethods.IsIconic(windowHandle))
                NativeMethods.ShowWindow(windowHandle, ShowWindowCommands.Restore);

            if (NativeMethods.SetForegroundWindow(windowHandle))
                return Ok();

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [OrcusGet("maximize")]
        public IActionResult Maximize(int processId)
        {
            if (!GetWindowHandle(processId, out var windowHandle, out var errorResult))
                return errorResult;

            if (NativeMethods.ShowWindow(windowHandle, ShowWindowCommands.Maximize))
                return Ok();

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [OrcusGet("minimize")]
        public IActionResult Minimize(int processId)
        {
            if (!GetWindowHandle(processId, out var windowHandle, out var errorResult))
                return errorResult;

            if (NativeMethods.ShowWindow(windowHandle, ShowWindowCommands.Minimize))
                return Ok();

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [OrcusGet("restore")]
        public IActionResult Restore(int processId)
        {
            if (!GetWindowHandle(processId, out var windowHandle, out var errorResult))
                return errorResult;

            if (NativeMethods.ShowWindow(windowHandle, ShowWindowCommands.Restore))
                return Ok();

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [OrcusGet("close")]
        public IActionResult Close(int processId)
        {
            if (!GetWindowHandle(processId, out var windowHandle, out var errorResult))
                return errorResult;

            if (NativeMethods.SendMessage(windowHandle, WM.CLOSE, IntPtr.Zero, IntPtr.Zero) == IntPtr.Zero)
                return Ok();

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        private bool GetWindowHandle(int processId, out IntPtr mainWindowHandle, out IActionResult errorResult)
        {
            try
            {
                using (var process = Process.GetProcessById(processId))
                    mainWindowHandle = process.MainWindowHandle;
            }
            catch (Exception)
            {
                errorResult = NotFound();
                mainWindowHandle = IntPtr.Zero;
                return false;
            }

            if (mainWindowHandle == IntPtr.Zero)
            {
                errorResult = StatusCode(StatusCodes.Status500InternalServerError);
                return false;
            }

            errorResult = null;
            return true;
        }
    }
}