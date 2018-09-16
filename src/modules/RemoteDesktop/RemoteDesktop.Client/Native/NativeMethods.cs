using System;
using System.Runtime.InteropServices;

namespace RemoteDesktop.Client.Native
{
    internal static class NativeMethods
    {
        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern unsafe IntPtr memcpy(byte* dst, byte* src, UIntPtr count);
    }
}