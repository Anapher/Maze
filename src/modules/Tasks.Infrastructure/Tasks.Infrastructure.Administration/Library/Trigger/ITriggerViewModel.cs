using Tasks.Infrastructure.Core.Triggers;

namespace Tasks.Infrastructure.Administration.Library.Trigger
{
    public interface ITriggerViewModel<TTriggerInfo> : ITaskServiceViewModel<TTriggerInfo> where TTriggerInfo : TriggerInfo
    {
    }
}