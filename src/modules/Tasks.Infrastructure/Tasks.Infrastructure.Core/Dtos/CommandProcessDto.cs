using System;

namespace Tasks.Infrastructure.Core.Dtos
{
    public class CommandProcessDto
    {
        public Guid CommandResultId { get; set; }
        public string TaskSessionHash { get; set; }
        public Guid TaskId { get; set; }
        public Guid TaskExecutionId { get; set; }

        public int? TargetId { get; set; }
        public double? Progress { get; set; }
        public string StatusMessage { get; set; }
    }
}
