using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using TaskManager.Client.Utilities;

namespace TaskManager.Client.ProcessInfo
{
    public class WmiValuesProvider : IProcessValueProvider
    {
        public IEnumerable<KeyValuePair<string, object>> ProvideValues(ManagementObject managementObject, Process process, bool updateProcess)
        {
            if (!updateProcess)
            {
                //these values cannot change
                if (process != null)
                    yield return new KeyValuePair<string, object>("Name", process.ProcessName);

                if (managementObject.TryGetDateTime("CreationDate", out var creationDate))
                    yield return new KeyValuePair<string, object>("CreationDate", creationDate);

                if (managementObject.TryGetProperty("ExecutablePath", out string executablePath))
                    yield return new KeyValuePair<string, object>("ExecutablePath", executablePath);

                if (managementObject.TryGetProperty("CommandLine", out string commandLine))
                    yield return new KeyValuePair<string, object>("CommandLine", commandLine);
            }

            if (managementObject.TryGetProperty("ParentProcessId", out uint parentProcessId))
                yield return new KeyValuePair<string, object>("ParentProcessId", parentProcessId);
        }
    }
}