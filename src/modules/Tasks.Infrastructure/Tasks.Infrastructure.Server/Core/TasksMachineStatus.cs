using System;
using System.Collections.Concurrent;
using Tasks.Infrastructure.Core.Dtos;

namespace Tasks.Infrastructure.Server.Core
{
    public class TasksMachineStatus
    {
        public TasksMachineStatus()
        {
            Processes = new ConcurrentDictionary<Guid, CommandProcessDto>();
            ActiveTasks = new ConcurrentDictionary<Guid, ActiveClientTaskDto>();
        }

        public ConcurrentDictionary<Guid, CommandProcessDto> Processes { get; }
        public ConcurrentDictionary<Guid, ActiveClientTaskDto> ActiveTasks { get; }
    }
}