using System;
using System.Collections.Generic;
using Tasks.Infrastructure.Core.Triggers;

namespace Tasks.Common.Triggers
{
    public class ScheduledTriggerInfo : TriggerInfo
    {
        public DateTimeOffset StartTime { get; set; }
        public ScheduleMode ScheduleMode { get; set; }
        public bool MonthlyAtRelativeDays { get; set; }
        public bool SynchronizeTimeZone { get; set; }
        public int RecurEvery { get; set; }
        public List<DayOfWeek> WeekDays { get; set; }
        public List<int> MonthDays { get; set; }
        public List<int> Months { get; set; }
        public List<RelativeDayInMonth> RelativeMonthDays { get; set; }
    }

    public enum ScheduleMode
    {
        Once,
        Daily,
        Weekly,
        Monthly
    }

    public enum RelativeDayInMonth
    {
        First,
        Second,
        Third,
        Fourth,
        Last
    }
}