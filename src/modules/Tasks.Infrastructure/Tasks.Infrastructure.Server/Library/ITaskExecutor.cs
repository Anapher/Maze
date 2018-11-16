using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Infrastructure.Core.Commands;

namespace Tasks.Infrastructure.Server.Library
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
        /// <param name="targetId">The target id the command should be executed on.</param>
        /// <param name="context">The context that provides the needed information and methods for the service.</param>
        /// <param name="cancellationToken">The cancellation token that should cancel the execution of the command. This is triggered when a <see cref="IStopService{TStopEventInfo}"/> completes.</param>
        /// <returns>Return the response of the command as a HTTP response.</returns>
        Task<HttpResponseMessage> InvokeAsync(TCommandInfo commandInfo, TargetId targetId, TaskExecutionContext context,
            CancellationToken cancellationToken);
    }
}