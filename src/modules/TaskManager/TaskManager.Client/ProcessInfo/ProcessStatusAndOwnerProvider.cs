using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Reflection;
using System.Security.Principal;
using TaskManager.Client.Native;
using TaskManager.Client.Utilities;
using TaskManager.Shared.Dtos;

namespace TaskManager.Client.ProcessInfo
{
    public class ProcessStatusAndOwnerProvider : IProcessValueProvider
    {
        public IEnumerable<KeyValuePair<string, object>> ProvideValues(ManagementObject managementObject, Process process, bool updateProcess)
        {
            var result = new List<KeyValuePair<string, object>>();
            try
            {
                var handle = process.Handle; //possible exception

                string sid = null;
                try
                {
                    sid = ProcessExtensions.GetProcessOwner(process.Handle);
                    var processOwner = new SecurityIdentifier(sid).Translate(typeof(NTAccount)).ToString();
                    result.Add(new KeyValuePair<string, object>("ProcessOwner", processOwner));
                }
                catch (Exception)
                {
                    // ignored
                }

                if (IsImmersiveProcess(handle))
                {
                    result.Add(new KeyValuePair<string, object>("Status", ProcessType.Immersive));
                    return result;
                }

                if (managementObject.TryGetProperty("SessionId", out uint sessionId) && sessionId == 0)
                {
                    result.Add(new KeyValuePair<string, object>("Status", ProcessType.Service));
                    return result;
                }

                if (IsNetAssembly(managementObject))
                {
                    result.Add(new KeyValuePair<string, object>("Status", ProcessType.NetAssembly));
                    return result;
                }

                if (sid != null && string.Equals(User.UserIdentity?.User?.Value, sid, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(new KeyValuePair<string, object>("Status", ProcessType.UserProcess));
                }
            }
            catch (Exception)
            {
                // no access to handle
            }

            return result;
        }

        private bool IsImmersiveProcess(IntPtr processHandle)
        {
            //IsImmersiveProcess is only available at win8+
            var win8Version = new Version(6, 2, 9200, 0);
            return Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version >= win8Version &&
                   NativeMethods.IsImmersiveProcess(processHandle);
        }

        private bool IsNetAssembly(ManagementObject managementObject)
        {
            if (managementObject.TryGetProperty("ExecutablePath", out string executablePath))
                try
                {
                    AssemblyName.GetAssemblyName(executablePath); //possible exception
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }

            return false;
        }
    }
}