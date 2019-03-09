using System.Collections.Generic;
using System.IO;
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

        public Task Compile(WixLightCompilationInfo compilationInfo, ILogger logger, CancellationToken cancellationToken)
        {
            var arguments = new List<ICommandLinePart>
            {
                new CliArgument("out", new CliPath(compilationInfo.OutputFilename)),
                new CliArgument("pdbout", new CliPath(compilationInfo.PdbOutputFilename))
            };

            if (compilationInfo.Extensions != null)
                arguments.AddRange(compilationInfo.Extensions.Select(x => new CliArgument("ext", x)));

            if (compilationInfo.LocalizationFiles != null)
                arguments.AddRange(compilationInfo.LocalizationFiles.Select(x => new CliArgument("loc", new CliPath(x))));

            if (compilationInfo.SuppressedICEs != null)
                arguments.AddRange(compilationInfo.SuppressedICEs.Select(x => new CliArgument("sice:" + x)));

            if (compilationInfo.ObjectFiles != null)
                arguments.AddRange(compilationInfo.ObjectFiles.Select(x => new CliPath(x)));

            return _toolRunner.Run(ToolName, logger, cancellationToken, arguments.ToArray());
        }
    }

    public class WixLightCompilationInfo
    {
        private string _pdbOutputFilename;

        public string OutputFilename { get; set; }
        public IEnumerable<string> Extensions { get; set; }
        public IEnumerable<string> LocalizationFiles { get; set; }
        public IEnumerable<string> SuppressedICEs { get; set; }
        public IEnumerable<string> ObjectFiles { get; set; }

        public string PdbOutputFilename
        {
            get =>
                _pdbOutputFilename ?? Path.Combine(Path.GetDirectoryName(OutputFilename),
                    Path.GetFileNameWithoutExtension(OutputFilename) + ".wixpdb");
            set => _pdbOutputFilename = value;
        }
    }
}