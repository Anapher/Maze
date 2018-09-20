using System;
using System.Runtime.InteropServices;

namespace RemoteDesktop.Administration.Native
{
    internal static class NativeMethods
    {
        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern IntPtr memcpy(IntPtr dest, IntPtr src, UIntPtr count);
    }
}