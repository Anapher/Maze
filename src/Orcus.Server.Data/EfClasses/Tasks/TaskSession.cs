using System;
using System.Collections.Generic;

namespace Orcus.Server.Data.EfClasses.Tasks
{
    public class TaskSession
    {
        public int TaskSessionId { get; set; }
        public int TaskReferenceId { get; set; }

        public string Description { get; set; }
        public string TaskSessionHash { get; set; }
        public DateTimeOffset CreatedOn { get; set; }

        public TaskReference TaskReference { get; set; }
        public ICollection<TaskTransmission> Transmissions { get; set; }
        public ICollection<TaskExecution> Executions { get; set; }
    }
}