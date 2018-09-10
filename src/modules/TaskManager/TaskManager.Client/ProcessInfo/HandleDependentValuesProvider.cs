using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Security.Principal;
using TaskManager.Client.Native;
using TaskManager.Client.Utilities;
using TaskManager.Shared.Dtos;

namespace TaskManager.Client.ProcessInfo
{
    public class HandleDependentValuesProvider : IProcessValueProvider
    {
        public void ProvideValue(ProcessDto processDto, Process process, ConcurrentDictionary<string, string> otherProperties, object dtoLock)
        {
            IntPtr handle;
            try
            {
                handle = process.Handle;
            }
            catch (Exception)
            {
                processDto.CanChangePriorityClass = false;
                return;
            }

            try
            {
                processDto.PriorityClass = process.PriorityClass;
                processDto.CanChangePriorityClass = true;
            }
            catch (Exception)
            {
                // ignored
            }

            try
            {
                var sid = ProcessExtensions.GetProcessOwner(handle);
                processDto.ProcessOwner = new SecurityIdentifier(sid).Translate(typeof(NTAccount)).ToString();
                if (string.Equals(User.UserIdentity?.User?.Value, sid, StringComparison.OrdinalIgnoreCase))
                    processDto.Status = ProcessStatus.UserProcess;
            }
            catch (Exception)
            {
                // ignored
            }

            try
            {
                processDto.ParentProcess = ProcessExtensions.GetParentProcess(handle);
            }
            catch (Exception)
            {
                // ignored
            }

            //IsImmersiveProcess is only available at win8+
            var win8Version = new Version(6, 2, 9200, 0);
            if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version >= win8Version &&
                NativeMethods.IsImmersiveProcess(handle))
                processDto.Status = ProcessStatus.Immersive;
        }
    }
}