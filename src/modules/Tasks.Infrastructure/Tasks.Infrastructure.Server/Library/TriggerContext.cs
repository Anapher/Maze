using System;
using System.Threading.Tasks;

namespace Tasks.Infrastructure.Server.Library
{
    public abstract class TriggerContext
    {
        /// <summary>
        ///     Create a new trigger session or return the existing session
        /// </summary>
        /// <param name="sessionKey">The unique session key</param>
        /// <returns>Return the session including methods to trigger it</returns>
        public abstract Task<TaskSessionTrigger> CreateSession(SessionKey sessionKey);
        public abstract Task<TaskSessionTrigger> CreateSession(SessionKey sessionKey, string description);

        public abstract Task<bool> IsClientIncluded(int clientId);
        public abstract bool IsServerIncluded();
        public abstract void ReportNextTrigger(DateTimeOffset dateTimeOffset);
    }
}