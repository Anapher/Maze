namespace Maze.Client.Administration.Core.Wix.Tools.Cli
{
    public class CliString : ICommandLineValue, ICommandLinePart
    {
        private readonly string _text;

        public CliString(string text)
        {
            _text = text;
        }

        public string GetArgument() => _text;
    }
}