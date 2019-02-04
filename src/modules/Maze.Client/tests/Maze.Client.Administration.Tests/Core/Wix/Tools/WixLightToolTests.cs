using System.Threading;
using System.Threading.Tasks;
using Maze.Client.Administration.Core.Wix.Tools;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Maze.Client.Administration.Tests.Core.Wix.Tools
{
    public class WixLightToolTests
    {
        private readonly MockWixToolRunner _runner;
        private readonly WixLightTool _tool;

        public WixLightToolTests()
        {
            _runner = new MockWixToolRunner();
            _tool = new WixLightTool(_runner);
        }

        [Fact]
        public async Task TestCompile()
        {
            await _tool.Compile("F:\\Projects\\CodeElements\\src\\CodeElements.Suite.Installer\\bin\\Debug\\en-us\\CodeElements.Suite.msi",
                "F:\\Projects\\CodeElements\\src\\CodeElements.Suite.Installer\\bin\\Debug\\en-us\\CodeElements.Suite.wixpdb",
                new[] {"WixUIExtension"}, new[] {"Common.wxl"},
                new[] {"obj\\Debug\\Components.Generated.wixobj", "C:\\test\\Program Files\\test.wixobj"}, NullLogger.Instance,
                CancellationToken.None);

            Assert.True(_runner.IsExecuted);

            const string commandLine =
                @"light.exe -out ""F:\Projects\CodeElements\src\CodeElements.Suite.Installer\bin\Debug\en-us\CodeElements.Suite.msi"" -pdbout ""F:\Projects\CodeElements\src\CodeElements.Suite.Installer\bin\Debug\en-us\CodeElements.Suite.wixpdb"" -ext WixUIExtension -loc ""Common.wxl"" ""obj\Debug\Components.Generated.wixobj"" ""C:\test\Program Files\test.wixobj""";
            Assert.Equal(commandLine, _runner.CommandLine);
        }
    }
}
