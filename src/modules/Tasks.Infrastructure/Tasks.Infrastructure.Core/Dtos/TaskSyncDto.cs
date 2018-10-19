using System;

namespace Tasks.Infrastructure.Core.Dtos
{
    public class TaskSyncDto
    {
        public Guid TaskId { get; set; }
        public string Hash { get; set; }
    }
}