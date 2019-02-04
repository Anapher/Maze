using System.Linq;

namespace Maze.Client.Administration.Core.Wix.Tools.Cli
{
    public class CliMultiple : ICommandLinePart
    {
        private readonly ICommandLineValue[] _parts;

        public CliMultiple(params ICommandLineValue[] parts)
        {
            _parts = parts;
        }

        public string GetArgument() => string.Join(" ", _parts.Select(x => x.GetArgument()));
    }
}