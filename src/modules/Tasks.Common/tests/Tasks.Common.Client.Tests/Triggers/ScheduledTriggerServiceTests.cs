using Orcus.Utilities;
using System;
using System.Collections.Generic;
using Tasks.Common.ServerClientShared.Triggers;
using Tasks.Common.Triggers;
using Xunit;

namespace Tasks.Common.Client.Tests.Triggers
{
    public class ScheduledTriggerServiceTests
    {
        public static readonly TheoryData<DateTime, DayOfWeek, DateTime> GetNextWeekdayTestData = new TheoryData<DateTime, DayOfWeek, DateTime> {
            {new DateTime(2018, 11, 27), DayOfWeek.Tuesday, new DateTime(2018, 11, 27)},
                        {new DateTime(2018, 11, 27), DayOfWeek.Friday, new DateTime(2018, 11, 30)},
                        {new DateTime(2018, 11, 27), DayOfWeek.Monday, new DateTime(2018, 12, 3)},
                         {new DateTime(2018, 11, 27, 1, 12, 17), DayOfWeek.Saturday, new DateTime(2018, 12, 1, 1, 12, 17)},
        };

            [Theory]
            [MemberData(nameof(GetNextWeekdayTestData))]
            public void TestGetNextWeekday(DateTime start, DayOfWeek day, DateTime expectedDay)
        {
            var result = ScheduledTriggerService.GetNextWeekday(start, day);
            Assert.Equal(expectedDay, result);
        }

        public static readonly TheoryData<DateTime, DateTime, IEnumerable<DayOfWeek>, DateTime?> GetNextWeekDateTestData = new TheoryData<DateTime, DateTime, IEnumerable<DayOfWeek>, DateTime?>
        {
            { new DateTime(2018, 11, 14), new DateTime(2018, 11, 13), DayOfWeek.Thursday.Yield(), new DateTime(2018, 11, 15)},
             {new DateTime(2018, 11, 28), new DateTime(2018, 11, 26, 19, 26, 8), new [] { DayOfWeek.Thursday, DayOfWeek.Monday }, new DateTime(2018, 11, 29, 19, 26, 8)},
             {new DateTime(2018, 11, 30), new DateTime(2018, 11, 26), new [] { DayOfWeek.Tuesday, DayOfWeek.Wednesday }, null},
             { new DateTime(2018, 11, 14, 14, 0, 0), new DateTime(2018, 11, 13, 14, 5, 0), DayOfWeek.Wednesday.Yield(), new DateTime(2018, 11, 14, 14, 5, 0)},
             { new DateTime(2018, 11, 14, 14, 10, 0), new DateTime(2018, 11, 13, 14, 5, 0), DayOfWeek.Wednesday.Yield(), null},
             { new DateTime(2018, 11, 14, 14, 10, 0), new DateTime(2018, 11, 13, 14, 5, 0), new [] { DayOfWeek.Wednesday, DayOfWeek.Thursday }, new DateTime(2018, 11, 15, 14, 5, 0)},
             { new DateTime(2018, 11, 14), new DateTime(2018, 11, 13), DayOfWeek.Monday.Yield(), new DateTime(2018, 11, 19)},
             { new DateTime(2018, 11, 19, 1, 1, 1), new DateTime(2018, 11, 14), DayOfWeek.Monday.Yield(), null},
        };

        [Theory]
        [MemberData(nameof(GetNextWeekDateTestData))]
        public void TestGetNextWeekDate(DateTime now, DateTime weekStart, IEnumerable<DayOfWeek> days, DateTime? expectedResult)
        {
            var result = ScheduledTriggerService.GetNextWeekDate(now, weekStart, days);
            Assert.Equal(expectedResult, result);
        }

        public static readonly TheoryData<int, int, RelativeDayInMonth, DayOfWeek, int> RelativeDateTestData = new TheoryData<int, int, RelativeDayInMonth, DayOfWeek, int>
        {
            { 2018, 11, RelativeDayInMonth.First, DayOfWeek.Monday, 5},
            { 2018, 12, RelativeDayInMonth.Second, DayOfWeek.Saturday, 8},
            { 2018, 11, RelativeDayInMonth.Third, DayOfWeek.Thursday, 15},
            { 2018, 11, RelativeDayInMonth.Third, DayOfWeek.Tuesday, 20},
            { 2018, 11, RelativeDayInMonth.Fourth, DayOfWeek.Friday, 23},
            { 2018, 11, RelativeDayInMonth.Last, DayOfWeek.Wednesday, 28},
            { 2018, 2, RelativeDayInMonth.First, DayOfWeek.Thursday, 1},
            { 2018, 2, RelativeDayInMonth.Second, DayOfWeek.Thursday, 8},
            { 2018, 2, RelativeDayInMonth.Third, DayOfWeek.Saturday, 17},
            { 2018, 2, RelativeDayInMonth.Last, DayOfWeek.Wednesday, 28},
            { 2018, 2, RelativeDayInMonth.Third, DayOfWeek.Tuesday, 20},
        };

        [Theory]
        [MemberData(nameof(RelativeDateTestData))]
        public void TestGetRelativeDate(int year, int month, RelativeDayInMonth relativeDay, DayOfWeek day, int expectedDay)
        {
            var result = ScheduledTriggerService.GetRelativeDate(year, month, relativeDay, day);
            Assert.Equal(year, result.Year);
            Assert.Equal(month, result.Month);
            Assert.Equal(expectedDay, result.Day);
        }
    }
}
