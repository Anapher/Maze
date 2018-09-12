using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using TaskManager.Client.Utilities;

namespace TaskManager.Client.ProcessInfo
{
    public class IconProvider : IProcessValueProvider
    {
        public IEnumerable<KeyValuePair<string, object>> ProvideValues(ManagementObject managementObject, Process process, bool updateProcess)
        {
            if (managementObject.TryGetProperty("ExecutablePath", out string executablePath))
                yield return new KeyValuePair<string, object>("Icon", FileUtilities.GetFileIcon(executablePath, 16));
        }
    }
}