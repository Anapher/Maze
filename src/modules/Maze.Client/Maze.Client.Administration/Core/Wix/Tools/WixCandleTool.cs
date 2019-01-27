using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Maze.Client.Administration.Core.Wix.Tools
{
    public class WixCandleTool : WixToolBase
    {
        public WixCandleTool(WixTools wixTools) : base(wixTools, "candle.xe")
        {
        }

        public Task Compile(ILogger logger, Dictionary<string, string> variables, IEnumerable<string> extensions, string outputPath,
            IEnumerable<string> files)
        {
            var arguments = new List<ICommandLinePart>();
            arguments.AddRange(variables.Select(x => new CliString($"-d{x.Key}=\"{x.Value}\"")));
            arguments.AddRange(extensions.Select(x => new CliArgument("ext", x)));
            arguments.Add(new CliArgument("out", new CliPath(outputPath)));
            arguments.AddRange(files.Select(x => new CliPath(x)));

            return Run(logger, arguments.ToArray());
        }
    }
}