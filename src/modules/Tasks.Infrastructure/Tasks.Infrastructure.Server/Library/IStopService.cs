using System.Threading;
using System.Threading.Tasks;
using Tasks.Infrastructure.Core.StopEvents;

namespace Tasks.Infrastructure.Server.Library
{
    /// <summary>
    ///     A service for <see cref="StopEventInfo"/>
    /// </summary>
    /// <typeparam name="TStopEventInfo">The data transfer object for this service</typeparam>
    public interface IStopService<in TStopEventInfo> where TStopEventInfo : StopEventInfo
    {
        /// <summary>
        ///     Invoke the stop event. This method is executed every time the task is triggered.
        /// </summary>
        /// <param name="stopEventInfo">The information for this stop event to configure the behavior.</param>
        /// <param name="context">The context that provides the needed information and methods for the service.</param>
        /// <param name="cancellationToken">The cancellation token that will be triggered if the task executed, was stopped or if the application shuts down.</param>
        /// <returns>Return a task that once it completed, the maze task will be stopped.</returns>
        Task InvokeAsync(TStopEventInfo stopEventInfo, StopContext context, CancellationToken cancellationToken);
    }

    public abstract class StopContext
    {
    }
}