using Xunit;

namespace RemoteDesktop.Shared.Tests
{
    public class ComponentOptionsTest
    {
        public static readonly TheoryData<ComponentOptions, string> TestData = new TheoryData<ComponentOptions, string>
        {
            {new ComponentOptions("x264") {Options = {{"test", "123"}}}, "x264 (test 123)"},
            {new ComponentOptions("x264") {Options = {{"bitrate", "3000k"}, {"nointerlace", null}, {"xs", "60"}}}, "x264 (bitrate 3000k, nointerlace, xs 60)"},
            {new ComponentOptions("xvid") {Options = {{"bitrate", "0,9"}, {"fps", "60"}}}, "xvid (bitrate \"0,9\", fps 60)"},
            {new ComponentOptions("avhd") {Options = {{"bitrate", "wtf is \"??"}, {"moringfilter", null}}}, "avhd (bitrate \"wtf is \\\"??\", moringfilter)"},
        };

        [Theory]
        [MemberData(nameof(TestData))]
        public void TestToString(ComponentOptions options, string expectedToString)
        {
            var toString = options.ToString();
            Assert.Equal(expectedToString, toString);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void TestParse(ComponentOptions expectedOptions, string toString)
        {
            var options = ComponentOptions.Parse(toString);
            Assert.Equal(expectedOptions.ComponentName, options.ComponentName);
            Assert.Equal(expectedOptions.Options, options.Options);
        }
    }
}