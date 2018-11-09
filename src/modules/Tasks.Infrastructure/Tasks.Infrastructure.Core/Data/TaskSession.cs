using System;
using System.Collections.Generic;

namespace Tasks.Infrastructure.Core.Data
{
    public class TaskSession
    {
        public string TaskSessionId { get; set; }
        public Guid TaskReferenceId { get; set; }

        public string Description { get; set; }
        public DateTimeOffset CreatedOn { get; set; }

        public TaskReference TaskReference { get; set; }
        public ICollection<TaskTransmission> Transmissions { get; set; }
        public ICollection<TaskExecution> Executions { get; set; }
    }
}