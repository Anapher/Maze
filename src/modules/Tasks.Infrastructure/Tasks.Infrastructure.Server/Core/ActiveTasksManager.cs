using System;
using System.Collections.Concurrent;
using Tasks.Infrastructure.Core.Dtos;
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

    public class TasksMachineStatus
    {
        public TasksMachineStatus()
        {
            Processes = new ConcurrentDictionary<Guid, CommandProcessDto>();
            ActiveTasks = new ConcurrentBag<ActiveClientTaskDto>();
        }

        public ConcurrentDictionary<Guid, CommandProcessDto> Processes { get; }
        public ConcurrentBag<ActiveClientTaskDto> ActiveTasks { get; }
    }
}