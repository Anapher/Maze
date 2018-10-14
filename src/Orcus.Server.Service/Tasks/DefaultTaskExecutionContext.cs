using System.Threading;
using Orcus.Server.Connection.Tasks;
using Orcus.Server.Data.EfClasses.Tasks;
using Orcus.Server.Library.Tasks;

namespace Orcus.Server.Service.Tasks
{
    public class DefaultTaskExecutionContext : TaskExecutionContext
    {
        public DefaultTaskExecutionContext(TaskSession session, OrcusTask orcusTask, CancellationToken cancellationToken)
        {
            Session = session;
            OrcusTask = orcusTask;
            CancellationToken = cancellationToken;
        }

        public override TaskSession Session { get; }
        public override OrcusTask OrcusTask { get; }
        public override CancellationToken CancellationToken { get; }
    }
}