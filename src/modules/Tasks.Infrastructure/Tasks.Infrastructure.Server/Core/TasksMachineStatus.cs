using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using Tasks.Infrastructure.Core.Dtos;

namespace Tasks.Infrastructure.Server.Core
{
    public class TasksMachineStatus
    {
        public TasksMachineStatus()
        {
            Processes = new ConcurrentDictionary<Guid, CommandProcessDto>();
        }

        public ConcurrentDictionary<Guid, CommandProcessDto> Processes { get; }
        public IImmutableList<ClientTaskDto> Tasks { get; set; }
    }
}