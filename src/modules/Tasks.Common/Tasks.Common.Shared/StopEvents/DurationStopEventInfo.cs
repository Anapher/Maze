using System;
using Tasks.Infrastructure.Core.StopEvents;

namespace Tasks.Common.Shared.StopEvents
{
    public class DurationStopEventInfo : StopEventInfo
    {
        public TimeSpan Duration { get; set; }
    }
}