using System.Collections.Concurrent;
using System.Diagnostics;
using TaskManager.Shared.Dtos;

namespace TaskManager.Client.ProcessInfo
{
    public class FileNameProvider : IProcessValueProvider
    {
        public void ProvideValue(ProcessDto processDto, Process process, ConcurrentDictionary<string, string> otherProperties, object dtoLock)
        {
            processDto.FileName = process.MainModule.FileName;
        }
    }
}