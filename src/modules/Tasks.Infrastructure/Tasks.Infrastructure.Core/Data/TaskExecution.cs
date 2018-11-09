using System;
using System.Collections.Generic;

namespace Tasks.Infrastructure.Core.Data
{
    public class TaskExecution
    {
        public Guid TaskExecutionId { get; set; }
        public string TaskSessionId { get; set; }

        public int? TargetId { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public IList<CommandResult> CommandResults { get; set; }
    }
}