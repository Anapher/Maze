using System.Threading;
using System.Threading.Tasks;
using Orcus.Server.Connection.Tasks.StopEvents;

namespace Orcus.Client.Library.Tasks
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