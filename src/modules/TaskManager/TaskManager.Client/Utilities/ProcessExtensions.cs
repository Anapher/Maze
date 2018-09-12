using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TaskManager.Client.Native;
using ProcessTreeInfo = System.Collections.Generic.Dictionary<int, (System.Diagnostics.Process process, int parentId)>;

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

        public static async Task<bool> KillProcessTree(int processId)
        {
            var processes = Process.GetProcesses();
            try
            {
                var processTree = new ProcessTreeInfo();

                foreach (var process in processes)
                {
                    try
                    {
                        var parentProcessId = GetParentProcess(process.Handle);
                        processTree.Add(process.Id, (process, parentProcessId));
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }

                if (!processTree.TryGetValue(processId, out var processInfo))
                    return false;

                //this only throws if the root process could not be killed
                await InternalKillProcessTree(processInfo.process, processTree);
                return true;
            }
            finally
            {
                foreach (var process in processes)
                    process.Dispose();
            }
        }

        private static async Task InternalKillProcessTree(Process process, ProcessTreeInfo processTree)
        {
            var task = process.KillGracefully();

            foreach (var childProcess in processTree.Where(x => x.Value.parentId == process.Id))
                try
                {
                    await InternalKillProcessTree(childProcess.Value.process, processTree);
                }
                catch (Exception)
                {
                    // ignored
                }

            await task;
        }

        public static void Suspend(this Process process)
        {
            if (process.ProcessName == string.Empty)
                return;

            foreach (ProcessThread pT in process.Threads)
            {
                IntPtr pOpenThread = NativeMethods.OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                if (pOpenThread == IntPtr.Zero)
                    continue;

                NativeMethods.SuspendThread(pOpenThread);
                NativeMethods.CloseHandle(pOpenThread);
            }
        }

        public static void Resume(this Process process)
        {
            if (process.ProcessName == string.Empty)
                return;

            foreach (ProcessThread pT in process.Threads)
            {
                if (pT.ThreadState != ThreadState.Wait || pT.WaitReason != ThreadWaitReason.Suspended)
                    continue;

                IntPtr pOpenThread = NativeMethods.OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                if (pOpenThread == IntPtr.Zero)
                {
                    continue;
                }

                int suspendCount;
                do
                {
                    suspendCount = NativeMethods.ResumeThread(pOpenThread);
                } while (suspendCount > 0);
                NativeMethods.CloseHandle(pOpenThread);
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

        public static async Task KillGracefully(this Process process)
        {
            process.EnableRaisingEvents = true;

            var succeedCloseMainWindow = false;
            try
            {
                if (process.MainWindowHandle != IntPtr.Zero)
                    succeedCloseMainWindow = process.CloseMainWindow();
            }
            catch (Exception)
            {
                // ignored
            }

            if (!process.HasExited)
            {
                if (succeedCloseMainWindow)
                {
                    var completionSource = new TaskCompletionSource<object>();
                    process.Exited += (sender, args) => completionSource.SetResult(null);

                    var resultedTask = await Task.WhenAny(completionSource.Task, Task.Delay(4000));
                    if (resultedTask == completionSource.Task)
                        return;
                }

                process.Kill();
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