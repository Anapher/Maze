using Tasks.Infrastructure.Core.StopEvents;

namespace Tasks.Infrastructure.Administration.Library.StopEvent
{
    /// <summary>
    /// The view model of a stop event (based on a <see cref="StopEventInfo"/> data transfer object)
    /// </summary>
    /// <typeparam name="TStopEvent">The data transfer object of the stop eent</typeparam>
    public interface IStopEventViewModel<TStopEvent> : ITaskServiceViewModel<TStopEvent> where TStopEvent : StopEventInfo
    {
    }
}