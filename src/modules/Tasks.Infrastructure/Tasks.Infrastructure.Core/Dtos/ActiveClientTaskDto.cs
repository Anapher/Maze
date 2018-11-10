using System;

namespace Tasks.Infrastructure.Core.Dtos
{
    public class ActiveClientTaskDto
    {
        public Guid TaskId { get; set; }
        public DateTimeOffset? NextTrigger { get; set; }
    }
}