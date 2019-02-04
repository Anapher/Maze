using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Maze.Client.Administration.Core.Wix.Tools.Cli;
using Microsoft.Extensions.Logging;

namespace Maze.Client.Administration.Core.Wix.Tools
{
    public class WixCandleTool
    {
        private readonly IWixToolRunner _toolRunner;
        private const string ToolName = "candle.exe";

        public WixCandleTool(IWixToolRunner toolRunner)
        {
            _toolRunner = toolRunner;
        }

        // C:\Program Files (x86)\WiX Toolset v3.11\bin\candle.exe -dBasePath=..\CodeElements.Suite\bin\Release\net47 -d"DevEnvDir=C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\\" -dSolutionDir=F:\Projects\CodeElements\ -dSolutionExt=.sln -dSolutionFileName=CodeElements.sln -dSolutionName=CodeElements -dSolutionPath=F:\Projects\CodeElements\CodeElements.sln -dConfiguration=Debug -dOutDir=bin\Debug\ -dPlatform=x86 -dProjectDir=F:\Projects\CodeElements\src\CodeElements.Suite.Installer\ -dProjectExt=.wixproj -dProjectFileName=CodeElements.Suite.Installer.wixproj -dProjectName=CodeElements.Suite.Installer -dProjectPath=F:\Projects\CodeElements\src\CodeElements.Suite.Installer\CodeElements.Suite.Installer.wixproj -dTargetDir=F:\Projects\CodeElements\src\CodeElements.Suite.Installer\bin\Debug\ -dTargetExt=.msi -dTargetFileName=CodeElements.Suite.msi -dTargetName=CodeElements.Suite -dTargetPath=F:\Projects\CodeElements\src\CodeElements.Suite.Installer\bin\Debug\CodeElements.Suite.msi -dCodeElements.Suite.Configuration=Debug -d"CodeElements.Suite.FullConfiguration=Debug|AnyCPU" -dCodeElements.Suite.Platform=AnyCPU -dCodeElements.Suite.ProjectDir=F:\Projects\CodeElements\src\CodeElements.Suite\ -dCodeElements.Suite.ProjectExt=.csproj -dCodeElements.Suite.ProjectFileName=CodeElements.Suite.csproj -dCodeElements.Suite.ProjectName=CodeElements.Suite -dCodeElements.Suite.ProjectPath=F:\Projects\CodeElements\src\CodeElements.Suite\CodeElements.Suite.csproj -dCodeElements.Suite.TargetDir=F:\Projects\CodeElements\src\CodeElements.Suite\bin\Debug\net47\ -dCodeElements.Suite.TargetExt=.exe -dCodeElements.Suite.TargetFileName=CodeElements.Suite.exe -dCodeElements.Suite.TargetName=CodeElements.Suite -dCodeElements.Suite.TargetPath=F:\Projects\CodeElements\src\CodeElements.Suite\bin\Debug\net47\CodeElements.Suite.exe -out obj\Debug\ -arch x86 -ext "C:\Program Files (x86)\WiX Toolset v3.11\bin\\WixUIExtension.dll" Components.Generated.wxs Components.wxs Directories.wxs Product.wxs
        public Task Compile(Dictionary<string, string> variables, IEnumerable<string> extensions, string outputPath, IEnumerable<string> files,
            ILogger logger, CancellationToken cancellationToken)
        {
            var arguments = new List<ICommandLinePart>();
            arguments.AddRange(variables.Select(x =>
            {
                if (x.Value.Contains(" "))
                    return new CliString($"-d\"{x.Key}={x.Value}\"");
                return new CliString($"-d{x.Key}={x.Value}");
            }));
            arguments.AddRange(extensions.Select(x => new CliArgument("ext", x)));
            arguments.Add(new CliArgument("out", new CliPath(outputPath + "\\")));
            arguments.AddRange(files.Select(x => new CliPath(x)));

            return _toolRunner.Run(ToolName, logger, cancellationToken, arguments.ToArray());
        }
    }
}