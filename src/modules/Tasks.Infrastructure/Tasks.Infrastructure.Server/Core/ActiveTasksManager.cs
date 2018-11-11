using System.Collections.Concurrent;
using Tasks.Infrastructure.Server.Library;

namespace Tasks.Infrastructure.Server.Core
{
    public class ActiveTasksManager
    {
        public ActiveTasksManager()
        {
            ActiveCommands = new ConcurrentDictionary<TargetId, TasksMachineStatus>();
        }

        public ConcurrentDictionary<TargetId, TasksMachineStatus> ActiveCommands { get; }
    }
}