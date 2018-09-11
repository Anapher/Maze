using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using TaskManager.Client.Utilities;

namespace TaskManager.Client.ProcessInfo
{
    public class FileVersionInfoProvider : IProcessValueProvider
    {
        public IEnumerable<KeyValuePair<string, object>> ProvideValues(ManagementObject managementObject, Process process, bool updateProcess)
        {
            if (!updateProcess)
                if (managementObject.TryGetProperty("ExecutablePath", out string executablePath))
                {
                    var fileVersionInfo = FileVersionInfo.GetVersionInfo(executablePath);

                    yield return new KeyValuePair<string, object>("Description", fileVersionInfo.FileDescription);
                    yield return new KeyValuePair<string, object>("CompanyName", fileVersionInfo.CompanyName);
                    yield return new KeyValuePair<string, object>("ProductVersion", fileVersionInfo.ProductVersion);
                    yield return new KeyValuePair<string, object>("FileVersion", fileVersionInfo.FileVersion);
                }
        }
    }
}