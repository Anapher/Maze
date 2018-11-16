using System.Threading.Tasks;
using Tasks.Infrastructure.Management.Data;

namespace Tasks.Infrastructure.Server.Library
{
    /// <summary>
    ///     The trigger for a task session
    /// </summary>
    public abstract class TaskSessionTrigger
    {
        /// <summary>
        ///     Execute the task on the given audience.
        /// </summary>
        /// <returns>Returns a task that completes once the command execution is completed.</returns>
        public abstract Task Invoke();

        /// <summary>
        ///     Execute the task on a specific client. If the client does not match the filter conditions, the task won't be executed.
        /// </summary>
        /// <param name="clientId">The id of the client that should execute the task.</param>
        /// <returns>Return a task that completes once the command execution is completed.</returns>
        public abstract Task InvokeClient(int clientId);

        /// <summary>
        ///     Execute the task on the server. If the server is not included in the audience, the task won't be executed.
        /// </summary>
        /// <returns>Return a task that completes once the command execution is completed.</returns>
        public abstract Task InvokeServer();

        /// <summary>
        ///     Information about the task session
        /// </summary>
        public abstract TaskSession Info { get; }
    }
}