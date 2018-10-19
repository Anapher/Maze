using System;
using System.Collections.Generic;

namespace Tasks.Infrastructure.Core.Data
{
    public class TaskReference
    {
        public int TaskReferenceId { get; set; }

        public Guid TaskId { get; set; }
        public string Filename { get; set; }
        public bool IsFinished { get; set; }
        public string Hash { get; set; }

        public ICollection<TaskSession> Sessions { get; set; }
    }
}