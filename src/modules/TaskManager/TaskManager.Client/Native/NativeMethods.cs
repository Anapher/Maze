using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

namespace TaskManager.Client.Native
{
    internal static class NativeMethods
    {
        [DllImport("ntdll.dll")]
        public static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass,
            ref ProcessBasicInformation processInformation, int processInformationLength, out int returnLength);

        [DllImport("advapi32")]
        public static extern bool OpenProcessToken(IntPtr processHandle, // handle to process
            int desiredAccess, // desired access to process
            ref IntPtr tokenHandle // handle to open access token
        );

        [DllImport("advapi32", CharSet = CharSet.Auto)]
        public static extern bool ConvertSidToStringSid(
            IntPtr pSid,
            [In, Out, MarshalAs(UnmanagedType.LPTStr)] ref string pStringSid
        );

        [DllImport("advapi32", CharSet = CharSet.Auto)]
        internal static extern bool GetTokenInformation(IntPtr hToken, TOKEN_INFORMATION_CLASS tokenInfoClass, IntPtr tokenInformation,
            int tokeInfoLength, ref int reqLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseHandle(IntPtr hObject);

        [DllImport("User32.dll")]
        internal static extern bool IsImmersiveProcess(IntPtr hProcess);
    }
}