using System;
using System.Collections.Generic;

namespace Tasks.Infrastructure.Management.Data
{
    public class TaskSession
    {
        public string TaskSessionId { get; set; }
        public Guid TaskReferenceId { get; set; }

        public string Description { get; set; }
        public DateTimeOffset CreatedOn { get; set; }

        public TaskReference TaskReference { get; set; }
        public IList<TaskTransmission> Transmissions { get; set; }
        public IList<TaskExecution> Executions { get; set; }
    }
}