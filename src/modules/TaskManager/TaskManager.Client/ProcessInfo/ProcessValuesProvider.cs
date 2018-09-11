using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;

namespace TaskManager.Client.ProcessInfo
{
    public class ProcessValuesProvider : IProcessValueProvider
    {
        public IEnumerable<KeyValuePair<string, object>> ProvideValues(ManagementObject managementObject, Process process, bool updateProcess)
        {
            yield return new KeyValuePair<string, object>("PrivateBytes", process.PrivateMemorySize64);
            yield return new KeyValuePair<string, object>("WorkingSet", process.WorkingSet64);
            yield return new KeyValuePair<string, object>("MainWindowHandle", (long)process.MainWindowHandle);

            ProcessPriorityClass priorityClass;
            try
            {
                priorityClass = process.PriorityClass;
            }
            catch (Exception)
            {
                yield break;
            }

            yield return new KeyValuePair<string, object>("PriorityClass", priorityClass);
        }
    }
}