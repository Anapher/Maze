using System.Collections.Concurrent;
using System.Diagnostics;
using TaskManager.Shared.Dtos;

namespace TaskManager.Client.ProcessInfo
{
    public class CoreValuesProvider : IProcessValueProvider
    {
        public void ProvideValue(ProcessDto processDto, Process process, ConcurrentDictionary<string, string> otherProperties, object dtoLock)
        {
            processDto.Id = process.Id;
            processDto.Name = process.ProcessName;
            processDto.PrivateBytes = process.PrivateMemorySize64;
            processDto.WorkingSet = process.WorkingSet64;
            processDto.MainWindowHandle = (long) process.MainWindowHandle;
        }
    }
}