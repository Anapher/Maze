using System;

namespace Orcus.Server.Data.EfClasses.Tasks
{
    public class TaskTransmission
    {
        public int TaskTransmissionId { get; set; }
        public int TaskSessionId { get; set; }

        public int? TargetId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}