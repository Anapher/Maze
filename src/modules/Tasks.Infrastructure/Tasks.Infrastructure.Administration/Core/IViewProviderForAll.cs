using Tasks.Infrastructure.Administration.Library.Command;
using Tasks.Infrastructure.Administration.Library.Filter;
using Tasks.Infrastructure.Administration.Library.StopEvent;
using Tasks.Infrastructure.Administration.Library.Trigger;

namespace Tasks.Infrastructure.Administration.Core
{
    public interface IViewProviderForAll : ICommandViewProvider, IFilterViewProvider, IStopEventViewProvider, ITriggerViewProvider { }
}