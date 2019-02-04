using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Maze.Client.Administration.Core.Wix.Tools.Cli;
using Microsoft.Extensions.Logging;

namespace Maze.Client.Administration.Core.Wix.Tools
{
    //C:\Program Files (x86)\WiX Toolset v3.11\bin\Light.exe -out F:\Projects\CodeElements\src\CodeElements.Suite.Installer\bin\Debug\en-us\CodeElements.Suite.msi -pdbout F:\Projects\CodeElements\src\CodeElements.Suite.Installer\bin\Debug\en-us\CodeElements.Suite.wixpdb -cultures:en-us -ext "C:\Program Files (x86)\WiX Toolset v3.11\bin\\WixUIExtension.dll" -loc Common.wxl -contentsfile obj\Debug\CodeElements.Suite.Installer.wixproj.BindContentsFileListen-us.txt -outputsfile obj\Debug\CodeElements.Suite.Installer.wixproj.BindOutputsFileListen-us.txt -builtoutputsfile obj\Debug\CodeElements.Suite.Installer.wixproj.BindBuiltOutputsFileListen-us.txt -wixprojectfile F:\Projects\CodeElements\src\CodeElements.Suite.Installer\CodeElements.Suite.Installer.wixproj obj\Debug\Components.Generated.wixobj obj\Debug\Components.wixobj obj\Debug\Directories.wixobj obj\Debug\Product.wixobj
    public class WixLightTool
    {
        private readonly IWixToolRunner _toolRunner;
        private const string ToolName = "light.exe";

        public WixLightTool(IWixToolRunner toolRunner)
        {
            _toolRunner = toolRunner;
        }

        public Task Compile(string outputFilename, string pdbOutputFilename, IEnumerable<string> extensions, IEnumerable<string> localizationFiles,
            IEnumerable<string> objFiles, ILogger logger, CancellationToken cancellationToken)
        {
            var arguments = new List<ICommandLinePart>
            {
                new CliArgument("out", new CliPath(outputFilename)), new CliArgument("pdbout", new CliPath(pdbOutputFilename))
            };
            arguments.AddRange(extensions.Select(x => new CliArgument("ext", x)));
            arguments.AddRange(localizationFiles.Select(x => new CliArgument("loc", new CliPath(x))));
            arguments.AddRange(objFiles.Select(x => new CliPath(x)));
            return _toolRunner.Run(ToolName, logger, cancellationToken, arguments.ToArray());
        }
    }
}