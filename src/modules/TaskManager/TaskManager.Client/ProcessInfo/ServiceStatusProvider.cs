using System.Collections.Concurrent;
using System.Diagnostics;
using TaskManager.Shared.Dtos;

namespace TaskManager.Client.ProcessInfo
{
    public class ServiceStatusProvider : IProcessValueProvider
    {
        public void ProvideValue(ProcessDto processDto, Process process, ConcurrentDictionary<string, string> otherProperties, object dtoLock)
        {
            lock (dtoLock)
            {
                //because Immersive > Service
                if (processDto.Status != ProcessStatus.Immersive)
                {
                    if (process.SessionId == 0)
                        processDto.Status = ProcessStatus.Service;
                }
            }
        }
    }
}