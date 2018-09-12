using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

namespace TaskManager.Client.Native
{
    internal static partial class NativeMethods
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, WM msg, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        internal static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

        [DllImport("kernel32.dll")]
        internal static extern uint SuspendThread(IntPtr hThread);

        [DllImport("kernel32.dll")]
        internal static extern uint ResumeThread(IntPtr hThread);

        /// <summary>
        /// TRUE if the process is running under WOW64. That is if it is a 32 bit process running on 64 bit Windows.
        /// If the process is running under 32-bit Windows, the value is set to FALSE. 
        /// If the process is a 64-bit application running under 64-bit Windows, the value is also set to FALSE.
        /// </summary>
        [DllImport("kernel32.dll")]
        internal static extern bool IsWow64Process(System.IntPtr aProcessHandle, out bool lpSystemInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseHandle(IntPtr hObject);
    }
}