using System;
using System.Collections.Generic;

namespace Tasks.Infrastructure.Core.Dtos
{
    public class ActiveClientTaskDto
    {
        public Guid TaskId { get; set; }
        public ActiveClientTaskStatus Status { get; set; }
        public Dictionary<string, Dictionary<string, string>> Sessions { get; set; }
        public DateTimeOffset? NextTrigger { get; set; }
    }

    public enum ActiveClientTaskStatus
    {
        Active,
        Executing
    }
}