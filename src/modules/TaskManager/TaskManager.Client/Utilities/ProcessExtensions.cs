using System;
using System.Runtime.InteropServices;
using TaskManager.Client.Native;

namespace TaskManager.Client.Utilities
{
    public static class ProcessExtensions
    {
        public static string GetProcessOwner(IntPtr handle)
        {
            try
            {
                var sid = string.Empty;
                if (DumpUserInfo(handle, out var sidHandle))
                    NativeMethods.ConvertSidToStringSid(sidHandle, ref sid);
                return sid;
            }
            catch (Exception)
            {
                return "Unknown";
            }
        }

        /// <summary>
        /// Gets the parent process of a specified process.
        /// </summary>
        /// <param name="handle">The process handle.</param>
        /// <returns>An instance of the Process class or null if an error occurred.</returns>
        //Source: https://stackoverflow.com/questions/394816/how-to-get-parent-process-in-net-in-managed-way
        public static int GetParentProcess(IntPtr handle)
        {
            var pbi = new ProcessBasicInformation();
            int returnLength;
            int status = NativeMethods.NtQueryInformationProcess(handle, 0, ref pbi, Marshal.SizeOf(pbi),
                out returnLength);
            if (status != 0)
                return -1;

            try
            {
                return pbi.InheritedFromUniqueProcessId.ToInt32();
            }
            catch (ArgumentException)
            {
                // not found
                return -1;
            }
        }

        private const int TOKEN_QUERY = 0X00000008;

        //Source: https://bytes.com/topic/c-sharp/answers/225065-how-call-win32-native-api-gettokeninformation-using-c
        public static bool DumpUserInfo(IntPtr pToken, out IntPtr sid)
        {
            int Access = TOKEN_QUERY;
            IntPtr procToken = IntPtr.Zero;
            bool ret = false;
            sid = IntPtr.Zero;
            try
            {
                if (NativeMethods.OpenProcessToken(pToken, Access, ref procToken))
                {
                    ret = ProcessTokenToSid(procToken, out sid);
                    NativeMethods.CloseHandle(procToken);
                }
                return ret;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool ProcessTokenToSid(IntPtr token, out IntPtr sid)
        {
            const int bufLength = 256;
            IntPtr tu = Marshal.AllocHGlobal(bufLength);
            sid = IntPtr.Zero;
            try
            {
                int cb = bufLength;
                var ret = NativeMethods.GetTokenInformation(token,
                    TOKEN_INFORMATION_CLASS.TokenUser, tu, cb, ref cb);
                if (ret)
                {
                    var tokUser = (TOKEN_USER)Marshal.PtrToStructure(tu, typeof(TOKEN_USER));
                    sid = tokUser.User.Sid;
                }
                return ret;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                Marshal.FreeHGlobal(tu);
            }
        }
    }
}