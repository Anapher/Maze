using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Maze.Client.Administration.Core.Wix.Tools;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Maze.Client.Administration.Tests.Core.Wix.Tools
{
    public class WixCandleToolTests
    {
        private readonly WixCandleTool _tool;
        private readonly MockWixToolRunner _runner;

        public WixCandleToolTests()
        {
            _runner = new MockWixToolRunner();
            _tool = new WixCandleTool(_runner);
        }

        [Fact]
        public async Task TestCompile()
        {
            await _tool.Compile(
                new Dictionary<string, string>
                {
                    {"BasePath", "C:\\My Folder"}, {"SolutionDir", "C:\\Documents\\Visual Studio 2021\\My Project"}, {"SolutionName", "Maze"}
                }, new[] { "WixUIExtension" }, "C:\\output\\path", new[] {"Components.Generated.wxs", "Components.wsx", "Directories.wsx"},
                NullLogger.Instance, CancellationToken.None);

            Assert.True(_runner.IsExecuted);

            const string commandLine =
                @"candle.exe -d""BasePath=C:\My Folder"" -d""SolutionDir=C:\Documents\Visual Studio 2021\My Project"" -dSolutionName=Maze -ext WixUIExtension -out ""C:\output\path"" ""Components.Generated.wxs"" ""Components.wsx"" ""Directories.wsx""";
            Assert.Equal(commandLine, _runner.CommandLine);
        }
    }
}
