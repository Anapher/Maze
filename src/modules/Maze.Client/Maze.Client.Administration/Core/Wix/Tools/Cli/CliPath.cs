namespace Maze.Client.Administration.Core.Wix.Tools.Cli
{
    public class CliPath : ICommandLineValue, ICommandLinePart
    {
        private readonly string _text;

        public CliPath(string text)
        {
            _text = text;
        }

        public string GetArgument()
        {
            if (_text.Contains(" "))
            {
                if (_text.EndsWith("\\"))
                    return $"\"{_text}\\\"";
                return $"\"{_text}\"";
            }

            return _text;
        }
    }
}