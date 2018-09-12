using System;
using System.Runtime.InteropServices;

namespace TaskManager.Client.Native
{
    internal static partial class NativeMethods
    {
        [DllImport("ntdll.dll")]
        public static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass,
            ref ProcessBasicInformation processInformation, int processInformationLength, out int returnLength);
    }
}