using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Core.Data;
using Tasks.Infrastructure.Server.Library;

namespace Tasks.Infrastructure.Server
{
    public class DefaultTaskExecutionContext : TaskExecutionContext
    {
        public DefaultTaskExecutionContext(TaskSession session, OrcusTask orcusTask)
        {
            Session = session;
            OrcusTask = orcusTask;
        }

        public override TaskSession Session { get; }
        public override OrcusTask OrcusTask { get; }
    }
}