using Tasks.Infrastructure.Core.Triggers;

namespace Tasks.Common.Shared.Triggers
{
    public class OnAppStartTriggerInfo : TriggerInfo
    {
        public bool OncePerDay { get; set; }
    }
}
