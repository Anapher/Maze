using FileExplorer.Shared.Utilities;
using Xunit;

namespace FileExplorer.Administration.Tests.Shared.Utilities
{
    public class PathHelperTests
    {
        public static readonly TheoryData<string, string[]> GetPathDirectoriesTestData =
            new TheoryData<string, string[]>
            {
                {"C:\\Windows\\System32", new[] {"C:\\", "C:\\Windows\\"}},
                {"C:\\", new string[0]},
            };

        [Theory]
        [MemberData(nameof(GetPathDirectoriesTestData))]
        public void TestGetPathDirectories(string path, string[] pathDirectories)
        {
            var result = PathHelper.GetPathDirectories(path);
            Assert.Equal(pathDirectories, result);
        }
    }
}