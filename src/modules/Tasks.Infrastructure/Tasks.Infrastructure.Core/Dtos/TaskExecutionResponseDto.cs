using System;

namespace Tasks.Infrastructure.Core.Dtos
{
   public class TaskExecutionResponseDto
    {
        public Guid TaskId { get; set; }
        public string SessionKey { get; set; }
        public string CommandName { get; set; }
        public string Result { get; set; }
        public int? Status { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}