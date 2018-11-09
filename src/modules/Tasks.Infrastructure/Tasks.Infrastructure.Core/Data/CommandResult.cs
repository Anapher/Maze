using System;

namespace Tasks.Infrastructure.Core.Data
{
    public class CommandResult
    {
        public Guid CommandResultId { get; set; }
        public Guid TaskExecutionId { get; set; }

        public string CommandName { get; set; }
        public string Result { get; set; }
        public int? Status { get; set; }
        public DateTimeOffset FinishedAt { get; set; }
    }
}
