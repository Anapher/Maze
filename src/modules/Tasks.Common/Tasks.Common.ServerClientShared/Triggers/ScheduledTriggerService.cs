using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Common.Triggers;

#if LIBRARY_CLIENT
using Tasks.Infrastructure.Client.Library;

#else
using Tasks.Infrastructure.Server.Library;

#endif

namespace Tasks.Common.ServerClientShared.Triggers
{
    public class ScheduledTriggerService : ITriggerService<ScheduledTriggerInfo>
    {
        public async Task InvokeAsync(ScheduledTriggerInfo triggerInfo, TriggerContext context, CancellationToken cancellationToken)
        {
            while (true)
            {
                var synchronizedTime = GetNextTriggerTime(triggerInfo, DateTimeOffset.UtcNow);
                if (synchronizedTime == null)
                    return;

                var nextTriggerTime = synchronizedTime.Value;
                if (!triggerInfo.SynchronizeTimeZone)
                {
                    //ignore offset
                    nextTriggerTime = new DateTimeOffset(nextTriggerTime.DateTime, DateTimeOffset.UtcNow.Offset);
                }

                context.ReportNextTrigger(nextTriggerTime);
                await Task.Delay(nextTriggerTime - DateTimeOffset.UtcNow, cancellationToken);

                //important: use synchronized time
                var session = await context.CreateSession(SessionKey.Create("ScheduledTrigger", synchronizedTime.Value));
                await session.Invoke();
            }
        }

        public static DateTimeOffset? GetNextTriggerTime(ScheduledTriggerInfo triggerInfo, DateTimeOffset now)
        {
            var diff = triggerInfo.StartTime - now;
            if (diff > TimeSpan.Zero)
                return triggerInfo.StartTime;

            switch (triggerInfo.ScheduleMode)
            {
                case ScheduleMode.Once:
                    return null;
                case ScheduleMode.Daily:
                    var periods = Math.Ceiling(diff.TotalDays / triggerInfo.RecurEvery);
                    return triggerInfo.StartTime.AddDays(periods * triggerInfo.RecurEvery);
                case ScheduleMode.Weekly:
                    var diffWeeks = diff.TotalDays / 7;
                    var period = Math.Floor(diffWeeks / triggerInfo.RecurEvery);

                    var currentPeriodStart = triggerInfo.StartTime.AddDays(period * triggerInfo.RecurEvery * 7);
                    var nextTrigger = GetNextWeekDate(now, currentPeriodStart, triggerInfo.WeekDays);
                    if (nextTrigger != null)
                        return nextTrigger.Value;

                    var nextPeriodStart = triggerInfo.StartTime.AddDays((period + 1) * triggerInfo.RecurEvery * 7);
                    return GetNextWeekDate(now, nextPeriodStart, triggerInfo.WeekDays) ?? throw new InvalidOperationException();
                case ScheduleMode.Monthly:
                    var nextTime = GetNextMonthlyDateTime(now, true, triggerInfo);
                    return nextTime ?? GetNextMonthlyDateTime(now, false, triggerInfo) ?? throw new InvalidOperationException();
            }

            throw new NotImplementedException();
        }

        public static DateTimeOffset? GetNextMonthlyDateTime(DateTimeOffset now, bool includeCurrent, ScheduledTriggerInfo triggerInfo)
        {
            var year = now.Year;
            var closestMonth = triggerInfo.Months.Select(x => (int?)x).Where(x => includeCurrent ? x >= now.Month : x > now.Month).OrderBy(x => x).FirstOrDefault();
            if (closestMonth == null)
            {
                closestMonth = triggerInfo.Months.OrderBy(x => x).First();
                year = now.Year + 1;
            }

            IEnumerable<int> days;
            if (triggerInfo.MonthlyAtRelativeDays)
            {
                var allDays = triggerInfo.WeekDays.SelectMany(x => triggerInfo.RelativeMonthDays.Select(y => GetRelativeDate(year, closestMonth.Value, y, x)));
                days = allDays.Select(x => x.Day);
            }
            else
            {
                days = triggerInfo.MonthDays;
            }

            var includeToday = triggerInfo.StartTime.TimeOfDay > now.TimeOfDay;
            var nextDay = days.Select(x => (int?)x).Where(x => includeToday ? x >= now.Day : x > now.Day).OrderBy(x => x).FirstOrDefault();
            if (nextDay == null)
                return null;

            return new DateTimeOffset(year, closestMonth.Value, nextDay.Value, triggerInfo.StartTime.Hour, triggerInfo.StartTime.Minute, triggerInfo.StartTime.Second, triggerInfo.StartTime.Offset);
        }

        public static DateTimeOffset? GetNextWeekDate(DateTimeOffset now, DateTimeOffset weekStartTime, IEnumerable<DayOfWeek> days)
        {
            var time = weekStartTime.TimeOfDay;

            return days.Select(day => (DateTimeOffset?) GetNextWeekday(weekStartTime.DateTime, day)).Where(x => x > now).OrderBy(x => x).FirstOrDefault();
        }

        public static DateTime GetRelativeDate(int year, int month, RelativeDayInMonth relativeDay, DayOfWeek day)
        {
            var monthBegin = new DateTime(year, month, 1);
            var firstDay = GetNextWeekday(monthBegin, day);

            switch (relativeDay)
            {
                case RelativeDayInMonth.First:
                    return firstDay;
                case RelativeDayInMonth.Second:
                    return firstDay.AddDays(7);
                case RelativeDayInMonth.Third:
                    return firstDay.AddDays(14);
                case RelativeDayInMonth.Fourth:
                    return firstDay.AddDays(21);
                case RelativeDayInMonth.Last:
                    var totalDays = DateTime.DaysInMonth(year, month);
                    return firstDay.AddDays(Math.Floor((totalDays - firstDay.Day) / 7d) * 7);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static DateTime GetNextWeekday(DateTime start, DayOfWeek day)
        {
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }
    }
}
