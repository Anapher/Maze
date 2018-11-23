using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.Infrastructure.Client.Library
{
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
        ///     A callback that is executed once the execution of this task finished
        /// </summary>
        public abstract Func<Task> AfterExecutionCallback { get; set; }

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
