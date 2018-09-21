using SystemInformation.Client.Utilities;
using Xunit;

namespace SystemInformation.Client.Tests.Utilities
{
    public class CamelSpaceExtensionsTests
    {
        [Theory]
        [InlineData("TEST", "TEST")]
        [InlineData("BurstSynchronousDRAM", "Burst Synchronous DRAM")]
        [InlineData("LinearFrameBuffer", "Linear Frame Buffer")]
        [InlineData("8514A", "8514A")]
        [InlineData("DSPProcessor", "DSP Processor")]
        [InlineData("Unknown", "Unknown")]
        [InlineData("CentralProcessor", "Central Processor")]
        public void TestFormatValue(string source, string expectedResult)
        {
            var result = source.SpaceCamelCase();
            Assert.Equal(expectedResult, result);
        }
    }
}