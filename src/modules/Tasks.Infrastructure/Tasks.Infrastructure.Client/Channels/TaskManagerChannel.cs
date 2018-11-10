using System;
using System.Threading.Tasks;
using Orcus.ControllerExtensions;

namespace Tasks.Infrastructure.Client.Channels
{
    public class TaskManagerChannel : CallTransmissionChannel<ITaskManagerChannel>, ITaskManagerChannel
    {
        public Task CreateOrUpdateTask(string serializedTask) => throw new NotImplementedException();

        public Task DeleteTask(Guid taskId) => throw new NotImplementedException();
    }
}
