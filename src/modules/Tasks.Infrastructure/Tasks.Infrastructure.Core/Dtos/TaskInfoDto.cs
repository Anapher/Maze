using System;
using System.Collections.Generic;

namespace Tasks.Infrastructure.Core.Dtos
{
    public class TaskInfoDto
    {
        public string Name { get; set; }    
        public Guid Id { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsCompletedOnServer { get; set; } //the task cannot complete on clients as new clients can always connect and the task may execute on them

        public int Commands { get; set; }
        public int TotalExecutions { get; set; }

        public List<string> Sessions { get; set; }
        public DateTimeOffset? NextExecution { get; set; }
        public DateTimeOffset? LastExecution { get; set; }
        public DateTimeOffset AddedOn { get; set; }
    }
}