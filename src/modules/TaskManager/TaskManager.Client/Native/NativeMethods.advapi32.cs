using System;
using System.Runtime.InteropServices;

namespace TaskManager.Client.Native
{
    internal static partial class NativeMethods
    {
        [DllImport("advapi32")]
        public static extern bool OpenProcessToken(IntPtr processHandle, // handle to process
            int desiredAccess, // desired access to process
            ref IntPtr tokenHandle // handle to open access token
        );

        [DllImport("advapi32", CharSet = CharSet.Auto)]
        public static extern bool ConvertSidToStringSid(IntPtr pSid, [In] [Out] [MarshalAs(UnmanagedType.LPTStr)]
            ref string pStringSid);

        [DllImport("advapi32", CharSet = CharSet.Auto)]
        internal static extern bool GetTokenInformation(IntPtr hToken, TOKEN_INFORMATION_CLASS tokenInfoClass, IntPtr tokenInformation,
            int tokeInfoLength, ref int reqLength);
    }
}