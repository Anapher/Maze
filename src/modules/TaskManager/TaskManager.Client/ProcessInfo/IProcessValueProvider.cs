using System.Collections.Concurrent;
using System.Diagnostics;
using TaskManager.Shared.Dtos;

namespace TaskManager.Client.ProcessInfo
{
    public interface IProcessValueProvider
    {
        void ProvideValue(ProcessDto processDto, Process process, ConcurrentDictionary<string, string> otherProperties, object dtoLock);
    }
}