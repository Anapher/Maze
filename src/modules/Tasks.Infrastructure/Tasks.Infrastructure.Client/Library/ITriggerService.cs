using System.Threading;
using System.Threading.Tasks;
using Tasks.Infrastructure.Core.Triggers;

namespace Tasks.Infrastructure.Client.Library
{
    /// <summary>
    ///     The service for <see cref="TTriggerInfo"/>
    /// </summary>
    /// <typeparam name="TTriggerInfo">The data transfer object for this service</typeparam>
    public interface ITriggerService<in TTriggerInfo> where TTriggerInfo : TriggerInfo
    {
        /// <summary>
        ///     Invoke the trigger. This method is executed once the task is enabled.
        /// </summary>
        /// <param name="triggerInfo">The information for this trigger to configure the behavior.</param>
        /// <param name="context">The context that provides the needed information and methods for the service.</param>
        /// <param name="cancellationToken">The cancellation token that will cancel the further trigger execution.</param>
        /// <returns>Return a task that once it completed, the trigger will be marked as completed and if all triggeres completed, the orcus task will be finished.</returns>
        Task InvokeAsync(TTriggerInfo triggerInfo, TriggerContext context, CancellationToken cancellationToken);
    }
}