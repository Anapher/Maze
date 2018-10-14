using Orcus.Server.Connection.Tasks;
using Orcus.Server.Data.EfClasses.Tasks;

namespace Orcus.Server.Library.Tasks
{
    public abstract class TaskExecutionContext
    {
        public abstract TaskSession Session { get; }
        public abstract OrcusTask OrcusTask { get; }
    }
}