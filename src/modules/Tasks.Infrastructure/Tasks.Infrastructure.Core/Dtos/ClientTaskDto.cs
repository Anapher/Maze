using System;

namespace Tasks.Infrastructure.Core.Dtos
{
    public class ClientTaskDto
    {
        public Guid TaskId { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset? NextTrigger { get; set; }
    }
}