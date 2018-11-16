using System;
using System.Threading.Tasks;

namespace Tasks.Infrastructure.Client.Library
{
    /// <summary>
    ///     The context that provides the needed information and methods for a trigger service
    /// </summary>
    public abstract class TriggerContext
    {
        /// <summary>
        ///     Create a new trigger session or return the existing session with the trigger servie name as description.
        /// </summary>
        /// <param name="sessionKey">The session key that identifies the session independently of the device.</param>
        /// <returns>Return the session including methods to trigger it</returns>
        public abstract Task<TaskSessionTrigger> CreateSession(SessionKey sessionKey);

        /// <summary>
        ///     Create a new trigger session or return the existing session.
        /// </summary>
        /// <param name="sessionKey">The session key that identifies the session independently of the device.</param>
        /// <param name="description">A short description of the session.</param>
        /// <returns>Return the session including methods to trigger it</returns>
        public abstract Task<TaskSessionTrigger> CreateSession(SessionKey sessionKey, string description);

        /// <summary>
        ///     Report the next time the service will trigger. This is only for showing to the user.
        /// </summary>
        /// <param name="dateTimeOffset">The date time when the trigger triggers next time.</param>
        public abstract void ReportNextTrigger(DateTimeOffset dateTimeOffset);
    }
}