using System.Threading;
using System.Threading.Tasks;
using Maze.Client.Administration.Core.Wix.Tools;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Maze.Client.Administration.Tests.Core.Wix.Tools
{
    public class WixHeatToolTests
    {
        private readonly MockWixToolRunner _runner;
        private readonly WixHeatTool _tool;

        public WixHeatToolTests()
        {
            _runner = new MockWixToolRunner();
            _tool = new WixHeatTool(_runner);
        }

        [Fact]
        public async Task TestGenerateComponent()
        {
            await _tool.GenerateComponent("C:\\test project\\bin\\Release", "ReleaseComponents", "INSTALLFOLDER", "var.BasePath",
                "Components.Generated.wxs", NullLogger.Instance, CancellationToken.None);

            Assert.True(_runner.IsExecuted);

            const string commandLine =
                @"heat.exe dir ""C:\test project\bin\Release"" -cg ReleaseComponents -dr INSTALLFOLDER -scom -sreg -srd -var var.BasePath -gg -sfrag -out ""Components.Generated.wxs""";
            Assert.Equal(commandLine, _runner.CommandLine);
        }
    }
}