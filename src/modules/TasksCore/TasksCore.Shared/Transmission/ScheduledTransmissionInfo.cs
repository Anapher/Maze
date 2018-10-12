using System;
using System.Collections.Generic;
using Orcus.Server.Connection.Tasks.Transmission;

namespace TasksCore.Shared.Transmission
{
    public class ScheduledTransmissionInfo : TransmissionInfo
    {
        public DateTimeOffset StartTime { get; set; }
        public ScheduleMode ScheduleMode { get; set; }
        public int RepetitionInterval { get; set; }
        public List<DayOfWeek> Days { get; set; }
    }

    public enum ScheduleMode
    {
        Once,
        Daily,
        Weekly,
        Monthly
    }
}
