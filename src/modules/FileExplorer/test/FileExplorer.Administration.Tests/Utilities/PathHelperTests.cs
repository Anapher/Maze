using FileExplorer.Shared.Utilities;
using Xunit;

namespace FileExplorer.Administration.Tests.Utilities
{
    public class PathHelperTests
    {
        [Theory]
        [InlineData("%USERNAME%")]
        [InlineData("C:\\%USERNAME%")]
        [InlineData("%appdata%\\Adobe")]
        [InlineData("%temp%")]
        [InlineData("C:\\Users\\%USERNAME%\\AppData\\Temp")]
        [InlineData("C:\\Users\\%USERNAME%HELLoWORLD")]
        public void TestPathsThatContainVariable(string path)
        {
            Assert.True(PathHelper.ContainsEnvironmentVariables(path));
        }

        [Theory]
        [InlineData("C:\\Users")]
        [InlineData("OneDrive")]
        [InlineData("%appdata")]
        [InlineData("C:\\10% on everything\\test")]
        [InlineData("C:\\10% on everything\\but only 5%")]
        public void TestPathsThatDontContainVariable(string path)
        {
            Assert.False(PathHelper.ContainsEnvironmentVariables(path));
        }
    }
}