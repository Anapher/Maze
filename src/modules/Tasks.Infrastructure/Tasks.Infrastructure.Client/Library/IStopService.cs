using System.Threading;
using System.Threading.Tasks;
using Tasks.Infrastructure.Core.StopEvents;

namespace Tasks.Infrastructure.Client.Library
{
    public interface IStopService<in TStopEventInfo> where TStopEventInfo : StopEventInfo
    {
        Task InvokeAsync(TStopEventInfo stopEventInfo, StopContext context, CancellationToken cancellationToken);
    }

    public abstract class StopContext
    {
        public abstract void Stop();
    }
}