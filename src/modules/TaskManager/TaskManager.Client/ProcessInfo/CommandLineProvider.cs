using System.Collections.Concurrent;
using System.Diagnostics;
using TaskManager.Client.Utilities;
using TaskManager.Shared.Dtos;

namespace TaskManager.Client.ProcessInfo
{
    public class CommandLineProvider : IProcessValueProvider
    {
        public void ProvideValue(ProcessDto processDto, Process process, ConcurrentDictionary<string, string> otherProperties, object dtoLock)
        {
            processDto.CommandLine = process.GetCommandLine();
        }
    }
}