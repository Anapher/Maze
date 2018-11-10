using System.Collections.Concurrent;

namespace Tasks.Infrastructure.Server.Core
{
    public interface ITasksConnectionManager
    {
        ConcurrentDictionary<int, ConnectedClientInfo> Clients { get; }
    }

    public class TasksConnectionManager : ITasksConnectionManager
    {
        public TasksConnectionManager()
        {
            Clients = new ConcurrentDictionary<int, ConnectedClientInfo>();
        }

        public ConcurrentDictionary<int, ConnectedClientInfo> Clients { get; }
    }
}