using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManager.Shared.Dtos;

namespace TaskManager.Shared.Channels
{
    public interface IProcessesProvider
    {
        Task<List<ChangeSet<ProcessDto>>> GetProcesses();
    }
}