using System;
using System.Globalization;
using FileExplorer.Administration.Converters;
using Xunit;

namespace FileExplorer.Administration.Tests.Converters
{
    public class TimeSpanToStringConverterTests
    {
        public static TheoryData<TimeSpan, string> TestData = new TheoryData<TimeSpan, string>
        {
            {TimeSpan.FromSeconds(5), "00:05"},
            {TimeSpan.FromMinutes(35.5), "35:30"},
            {TimeSpan.FromHours(1.5), "01:30:00"},
            {new TimeSpan(0, 5, 37, 12), "05:37:12"},
            {new TimeSpan(0, 26, 37, 12), "26:37:12"},
            {new TimeSpan(5, 26, 37, 12), "146:37:12"},
        };

        [Theory]
        [MemberData(nameof(TestData))]
        public void TestConvertToString(TimeSpan value, string expected)
        {
            var converter = new TimeSpanToStringConverter();
            var result = (string) converter.Convert(value, typeof(string), null, CultureInfo.InvariantCulture);
            Assert.Equal(result, expected);
        }
    }
}