using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Orcus.Server.Library.Tasks;
using TasksCore.Shared.Transmission;

namespace TasksCore.Server.Transmission
{
    public class ScheduledTransmissionService : ITransmissionService<ScheduledTransmissionInfo>
    {
        public async Task InvokeAsync(ScheduledTransmissionInfo transmissionInfo, TransmissionContext context, CancellationToken cancellationToken)
        {
            while (await InternalInvokeAsync(transmissionInfo, context, cancellationToken))
            {
                var session = await context.GetSession("Scheduled Transmission");
                await session.InvokeAll();
            }
        }

        public async Task<bool> InternalInvokeAsync(ScheduledTransmissionInfo transmissionInfo, TransmissionContext context,
            CancellationToken cancellationToken)
        {
            var diff = transmissionInfo.StartTime - DateTimeOffset.UtcNow;
            if (diff > TimeSpan.Zero)
            {
                await Task.Delay(diff, cancellationToken);
                return true;
            }
            
            switch (transmissionInfo.ScheduleMode)
            {
                case ScheduleMode.Once:
                    return false;
                case ScheduleMode.Daily:
                    var days = (DateTimeOffset.UtcNow - transmissionInfo.StartTime).TotalDays;
                    diff = TimeSpan.FromDays(transmissionInfo.RepetitionInterval - (days % (transmissionInfo.RepetitionInterval)));
                    await Task.Delay(diff, cancellationToken);
                    return true;
                case ScheduleMode.Weekly:
                    var now = DateTimeOffset.UtcNow;
                    var weekTimeSpan = TimeSpan.FromDays(transmissionInfo.RepetitionInterval * 7);
                    var daysSinceStart = (now - transmissionInfo.StartTime).TotalDays;
                    var currentWeekOffset = Math.Floor(daysSinceStart / weekTimeSpan.TotalDays);
                    var currentWeek = transmissionInfo.StartTime.AddDays(weekTimeSpan.TotalDays * currentWeekOffset);

                    var timeOfTheDay = (transmissionInfo.StartTime - transmissionInfo.StartTime.Date).TotalHours;
                    var weekdayQueue =
                        new Queue<DateTime>(transmissionInfo.Days.Select(x => GetNextWeekday(currentWeek.DateTime, x).AddHours(timeOfTheDay))
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
