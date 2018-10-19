using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Orcus.Server.Library.Tasks;
using TasksCore.Services.Shared.Transmission;

namespace TasksCore.Server.Transmission
{
    public class ScheduledTriggerService : ITriggerService<ScheduledTriggerInfo>
    {
        public async Task InvokeAsync(ScheduledTriggerInfo triggerInfo, TriggerContext context, CancellationToken cancellationToken)
        {
            while (await InternalInvokeAsync(triggerInfo, context, cancellationToken))
            {
                var session = await context.CreateSession(SessionKey.Create("Scheduled", DateTimeOffset.UtcNow));
                await session.InvokeAll();
            }
        }

        public async Task<bool> InternalInvokeAsync(ScheduledTriggerInfo triggerInfo, TriggerContext context,
            CancellationToken cancellationToken)
        {
            var diff = triggerInfo.StartTime - DateTimeOffset.UtcNow;
            if (diff > TimeSpan.Zero)
            {
                await Task.Delay(diff, cancellationToken);
                return true;
            }
            
            switch (triggerInfo.ScheduleMode)
            {
                case ScheduleMode.Once:
                    return false;
                case ScheduleMode.Daily:
                    var days = (DateTimeOffset.UtcNow - triggerInfo.StartTime).TotalDays;
                    diff = TimeSpan.FromDays(triggerInfo.RepetitionInterval - (days % (triggerInfo.RepetitionInterval)));
                    await Task.Delay(diff, cancellationToken);
                    return true;
                case ScheduleMode.Weekly:
                    var now = DateTimeOffset.UtcNow;
                    var weekTimeSpan = TimeSpan.FromDays(triggerInfo.RepetitionInterval * 7);
                    var daysSinceStart = (now - triggerInfo.StartTime).TotalDays;
                    var currentWeekOffset = Math.Floor(daysSinceStart / weekTimeSpan.TotalDays);
                    var currentWeek = triggerInfo.StartTime.AddDays(weekTimeSpan.TotalDays * currentWeekOffset);

                    var timeOfTheDay = (triggerInfo.StartTime - triggerInfo.StartTime.Date).TotalHours;
                    var weekdayQueue =
                        new Queue<DateTime>(triggerInfo.Days.Select(x => GetNextWeekday(currentWeek.DateTime, x).AddHours(timeOfTheDay))
                            .OrderBy(x => x));
                    var nextTime = weekdayQueue.Dequeue();

                    while (now > nextTime)
                    {
                        var fixedTime = nextTime.AddDays(weekTimeSpan.TotalDays);
                        weekdayQueue.Enqueue(fixedTime);

                        nextTime = weekdayQueue.Dequeue();
                    }

                    await Task.Delay(nextTime - now, cancellationToken);
                    return true;
                case ScheduleMode.Monthly:
                    return false;
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
