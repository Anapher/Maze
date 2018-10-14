using System;

namespace Orcus.Server.Data.EfClasses.Tasks
{
    public class TaskExecution
    {
        public int TaskExecutionId { get; set; }
        public int TaskSessionId { get; set; }

        public int? TargetId { get; set; }
        public string CommandName { get; set; }
        public string Result { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}