using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Maze.Client.Administration.Core.Wix.Tools
{
    public abstract class WixToolBase
    {
        private readonly WixTools _wixTools;
        private readonly string _toolName;

        protected WixToolBase(WixTools wixTools, string toolName)
        {
            _wixTools = wixTools;
            _toolName = toolName;
        }

        protected Task Run(ILogger logger, params ICommandLinePart[] parts)
        {
            var arguments = string.Join(" ", parts.Select(x => x.GetArgument()));
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = Path.Combine(_wixTools.Directory, _toolName),
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            });

            process.WaitForExit();
        }
    }

    public interface ICommandLinePart
    {
        string GetArgument();
    }

    public interface ICommandLineValue
    {
        string GetArgument();
    }

    public class CliString : ICommandLineValue, ICommandLinePart
    {
        private readonly string _text;

        public CliString(string text)
        {
            _text = text;
        }

        public string GetArgument() => _text;
    }

    public class CliMultiple : ICommandLinePart
    {
        private readonly ICommandLineValue[] _parts;

        public CliMultiple(params ICommandLineValue[] parts)
        {
            _parts = parts;
        }

        public string GetArgument() => string.Join(" ", _parts.Select(x => x.GetArgument()));
    }

    public class CliPath : ICommandLineValue, ICommandLinePart
    {
        private readonly string _text;

        public CliPath(string text)
        {
            _text = text;
        }

        public string GetArgument() => $"\"{_text}\"";
    }

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
