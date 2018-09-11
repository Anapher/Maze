using System.Collections.Generic;
using System.Diagnostics;
using System.Management;

namespace TaskManager.Client.ProcessInfo
{
    public interface IProcessValueProvider
    {
        IEnumerable<KeyValuePair<string, object>> ProvideValues(ManagementObject managementObject, Process process, bool updateProcess);
    }
}