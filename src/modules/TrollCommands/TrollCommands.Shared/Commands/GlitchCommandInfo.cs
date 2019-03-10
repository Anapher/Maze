using Tasks.Infrastructure.Core.Commands;

namespace TrollCommands.Shared.Commands
{
    public class GlitchCommandInfo : CommandInfo
    {
        public int Power { get; set; }
        public int MaxSize { get; set; }
        public int Interval { get; set; }
    }
}