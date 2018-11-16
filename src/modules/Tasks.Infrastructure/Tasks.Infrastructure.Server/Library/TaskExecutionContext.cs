using System;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Management.Data;

namespace Tasks.Infrastructure.Server.Library
{
    /// <summary>
    ///     Provide information about the execution proess of a command
    /// </summary>
    public abstract class TaskExecutionContext
    {
        /// <summary>
        ///     The session this command is executed on
        /// </summary>
        public abstract TaskSession Session { get; }

        /// <summary>
        ///     The task that contains the command.
        /// </summary>
        public abstract OrcusTask OrcusTask { get; }

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