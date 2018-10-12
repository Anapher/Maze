using System;
using System.Collections.Generic;

namespace Orcus.Server.Data.EfClasses.Tasks
{
    public class CapturedTask
    {
        public int CapturedTaskId { get; set; }

        public Guid TaskId { get; set; }
        public string Filename { get; set; }
        public bool IsFinished { get; set; }

        public ICollection<TaskSession> Sessions { get; set; }
    }
}
