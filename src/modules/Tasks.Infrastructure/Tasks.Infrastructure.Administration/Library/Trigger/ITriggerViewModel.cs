using Tasks.Infrastructure.Core.Triggers;

namespace Tasks.Infrastructure.Administration.Library.Trigger
{
    /// <summary>
    /// The view model of a trigger (based on a <see cref="TriggerInfo"/> data transfer object)
    /// </summary>
    /// <typeparam name="TTriggerInfo">The data transfer object of the trigger</typeparam>
    public interface ITriggerViewModel<TTriggerInfo> : ITaskServiceViewModel<TTriggerInfo> where TTriggerInfo : TriggerInfo
    {
    }
}