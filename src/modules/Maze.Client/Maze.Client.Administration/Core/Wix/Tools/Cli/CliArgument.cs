namespace Maze.Client.Administration.Core.Wix.Tools.Cli
{
    public class CliArgument : ICommandLinePart
    {
        private readonly string _argument;
        private readonly ICommandLineValue _value;

        public CliArgument(string argument)
        {
            _argument = argument;
        }

        public CliArgument(string argument, ICommandLineValue value)
        {
            _argument = argument;
            _value = value;
        }

        public CliArgument(string argument, string value)
        {
            _argument = argument;
            _value = new CliString(value);
        }

        public string GetArgument()
        {
            var result = $"-{_argument}";
            if (_value != null)
                result += " " + _value.GetArgument();

            return result;
        }
    }
}