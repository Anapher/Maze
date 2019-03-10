using Tasks.Infrastructure.Core.Commands;

namespace TrollCommands.Shared.Commands
{
    public class EarthquakeCommandInfo : CommandInfo
    {
        public int Interval { get; set; }
        public int Power { get; set; }
    }
}