using System;
using System.Threading.Tasks;
using TaskManager.Shared.Dtos;

namespace TaskManager.Shared.Channels
{
    public interface IProcessWatcher
    {
        event EventHandler<ProcessExitedEventArgs> Exited;

        Task WatchProcess(int processId);
        Task<ChangingProcessPropertiesDto> GetInfo();
    }

    public class ProcessExitedEventArgs
    {
        public int ExitCode { get; set; }
    }
}