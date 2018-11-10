using System;
using System.Threading.Tasks;

namespace Tasks.Infrastructure.Management.Channels
{
    public interface ITaskManagerChannel
    {
        Task CreateOrUpdateTask(string serializedTask);
        Task DeleteTask(Guid taskId);
    }
}