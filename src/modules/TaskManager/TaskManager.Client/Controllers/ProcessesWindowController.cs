using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Maze.Modules.Api;
using Maze.Modules.Api.Routing;
using TaskManager.Client.Native;

namespace TaskManager.Client.Controllers
{
    [Route("processes/{processId}/window")]
    public class ProcessesWindowController : MazeController
    {
        [MazeGet("bringToFront")]
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

        [MazeGet("maximize")]
        public IActionResult Maximize(int processId)
        {
            if (!GetWindowHandle(processId, out var windowHandle, out var errorResult))
                return errorResult;

            if (NativeMethods.ShowWindow(windowHandle, ShowWindowCommands.Maximize))
                return Ok();

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [MazeGet("minimize")]
        public IActionResult Minimize(int processId)
        {
            if (!GetWindowHandle(processId, out var windowHandle, out var errorResult))
                return errorResult;

            if (NativeMethods.ShowWindow(windowHandle, ShowWindowCommands.Minimize))
                return Ok();

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [MazeGet("restore")]
        public IActionResult Restore(int processId)
        {
            if (!GetWindowHandle(processId, out var windowHandle, out var errorResult))
                return errorResult;

            if (NativeMethods.ShowWindow(windowHandle, ShowWindowCommands.Restore))
                return Ok();

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [MazeGet("close")]
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