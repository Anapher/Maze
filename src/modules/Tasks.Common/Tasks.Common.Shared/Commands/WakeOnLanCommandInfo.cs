using Tasks.Infrastructure.Core.Commands;

namespace Tasks.Common.Shared.Commands
{
    public class WakeOnLanCommandInfo : CommandInfo
    {
        public bool TryOverClient { get; set; }
    }
}