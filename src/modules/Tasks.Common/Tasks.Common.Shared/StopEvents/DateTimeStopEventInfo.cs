using System;
using Tasks.Infrastructure.Core.StopEvents;

namespace Tasks.Common.Shared.StopEvents
{
    public class DateTimeStopEventInfo : StopEventInfo
    {
        public DateTimeOffset DateTime { get; set; }
    }
}