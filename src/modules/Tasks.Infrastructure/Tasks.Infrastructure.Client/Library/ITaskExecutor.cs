using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Infrastructure.Core.Commands;

namespace Tasks.Infrastructure.Client.Library
{
    public interface ITaskExecutor<in TCommandInfo> where TCommandInfo : CommandInfo
    {
        Task<HttpResponseMessage> InvokeAsync(TCommandInfo commandInfo, TaskExecutionContext context, CancellationToken cancellationToken);
    }

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