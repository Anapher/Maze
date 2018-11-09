using System;
using System.Collections.Generic;

namespace Tasks.Infrastructure.Core.Data
{
    public class TaskReference
    {
        public Guid TaskId { get; set; }

        public bool IsCompleted { get; set; }

        public ICollection<TaskSession> Sessions { get; set; }
    }
}