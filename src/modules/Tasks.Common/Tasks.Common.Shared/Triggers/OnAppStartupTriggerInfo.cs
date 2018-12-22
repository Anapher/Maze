using Tasks.Infrastructure.Core.Triggers;

namespace Tasks.Common.Triggers
{
    public class OnAppStartupTriggerInfo : TriggerInfo
    {
        public bool OncePerDay { get; set; }
    }
}
