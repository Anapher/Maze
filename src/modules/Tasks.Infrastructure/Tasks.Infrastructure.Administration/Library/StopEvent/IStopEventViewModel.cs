using Tasks.Infrastructure.Core.StopEvents;

namespace Tasks.Infrastructure.Administration.Library.StopEvent
{
    public interface IStopEventViewModel<TStopEvent> : ITaskServiceViewModel<TStopEvent> where TStopEvent : StopEventInfo
    {
    }
}