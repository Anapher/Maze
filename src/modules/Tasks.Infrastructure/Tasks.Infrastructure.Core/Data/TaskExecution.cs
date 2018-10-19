using System;

namespace Tasks.Infrastructure.Core.Data
{
    public class TaskExecution
    {
        public int TaskExecutionId { get; set; }
        public int TaskSessionId { get; set; }

        public int? TargetId { get; set; }
        public string CommandName { get; set; }
        public string Result { get; set; }
        public string ExecutionError { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}