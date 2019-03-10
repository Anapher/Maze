using Tasks.Infrastructure.Core.Commands;

namespace TrollCommands.Shared.Commands
{
    public class CrazyCursorCommandInfo : CommandInfo
    {
        public int Delay { get; set; }
        public int Power { get; set; }
    }
}