using System.Threading;
using System.Threading.Tasks;
using Maze.Client.Administration.Core.Wix.Tools.Cli;
using Microsoft.Extensions.Logging;

namespace Maze.Client.Administration.Core.Wix.Tools
{
    public class WixHeatTool
    {
        private readonly IWixToolRunner _toolRunner;
        private const string ToolName = "heat.exe";

        public WixHeatTool(IWixToolRunner toolRunner)
        {
            _toolRunner = toolRunner;
        }

        public Task GenerateComponent(string directoryPath, string componentGroup, string directoryReference, string basePathVariable, string outPath,
            ILogger logger, CancellationToken cancellationToken)
        {
            // C:\Program Files (x86)\WiX Toolset v3.11\bin\Heat.exe dir ..\CodeElements.Suite\bin\Release\net47 -cg ReleaseComponents -dr INSTALLFOLDER -scom -sreg -srd -var var.BasePath -gg -sfrag -out Components.Generated.wxs
            return _toolRunner.Run(ToolName, logger, cancellationToken, new CliString("dir"), new CliPath(directoryPath),
                new CliArgument("cg", componentGroup), new CliArgument("dr", directoryReference), new CliArgument("scom"), new CliArgument("sreg"),
                new CliArgument("srd"), new CliArgument("var", basePathVariable), new CliArgument("gg"), new CliArgument("sfrag"),
                new CliArgument("out", new CliPath(outPath)));
        }
    }
}
