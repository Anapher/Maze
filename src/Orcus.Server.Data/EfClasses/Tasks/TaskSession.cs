using System.Collections.Generic;

namespace Orcus.Server.Data.EfClasses.Tasks
{
    public class TaskSession
    {
        public int TaskSessionId { get; set; }
        public int CapturedTaskId { get; set; }

        public string Name { get; set; }
        public string SourceTrigger { get; set; }

        public CapturedTask CapturedTask { get; set; }
        public ICollection<TaskTransmission> Transmissions { get; set; }
        public ICollection<TaskExecution> Executions { get; set; }
    }
}