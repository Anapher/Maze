using System;
using System.Runtime.InteropServices;

namespace TaskManager.Client.Native
{
    internal static partial class NativeMethods
    {
        [DllImport("User32.dll")]
        internal static extern bool IsImmersiveProcess(IntPtr hProcess);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommands nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}