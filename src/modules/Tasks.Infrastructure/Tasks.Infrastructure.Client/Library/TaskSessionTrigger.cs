using System.Threading.Tasks;
using Tasks.Infrastructure.Management.Data;

namespace Tasks.Infrastructure.Client.Library
{
    /// <summary>
    ///     A task session trigger
    /// </summary>
    public abstract class TaskSessionTrigger
    {
        /// <summary>
        ///     Invoke the trigger on the session
        /// </summary>
        /// <returns>Return a task that once it completed, all commands have executed.</returns>
        public abstract Task Invoke();

        /// <summary>
        ///     Provide information about the session
        /// </summary>
        public abstract TaskSession Info { get; }
    }
}