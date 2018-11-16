using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Infrastructure.Core.Commands;

namespace Tasks.Infrastructure.Client.Library
{
    /// <summary>
    ///     The service for <see cref="CommandInfo"/>
    /// </summary>
    /// <typeparam name="TCommandInfo">The data transfer object for this service</typeparam>
    public interface ITaskExecutor<in TCommandInfo> where TCommandInfo : CommandInfo
    {
        /// <summary>
        ///     Invoke the command.
        /// </summary>
        /// <param name="commandInfo">The information for this command to configure the behavior.</param>
        /// <param name="context">The context that provides the needed information and methods for the service.</param>
        /// <param name="cancellationToken">The cancellation token that should cancel the execution of the command. This is triggered when a <see cref="IStopService{TStopEventInfo}"/> completes.</param>
        /// <returns>Return the response of the command as a HTTP response.</returns>
        Task<HttpResponseMessage> InvokeAsync(TCommandInfo commandInfo, TaskExecutionContext context, CancellationToken cancellationToken);
    }

    /// <summary>
    ///     The context that provides the needed information and methods for a task executor
    /// </summary>
    public abstract class TaskExecutionContext
    {
        /// <summary>
        ///     Receive services from the global services
        /// </summary>
        public abstract IServiceProvider Services { get; }

        /// <summary>
        ///     Report a process status message. The status may be sent delayed and there is no gurantee that it will be actually sent at all.
        /// </summary>
        /// <param name="message">The status message that describes the current operation</param>
        public abstract void ReportStatus(string message);

        /// <summary>
        ///     Report a progress status (0-1, null for indeterminate). The status may be sent delayed and there is no gurantee that it will be actually sent at all.
        /// </summary>
        /// <param name="progress"></param>
        public abstract void ReportProgress(double? progress);
    }
}