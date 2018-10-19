using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Core.Data;

namespace Tasks.Infrastructure.Server.Library
{
    public abstract class TaskExecutionContext
    {
        public abstract TaskSession Session { get; }
        public abstract OrcusTask OrcusTask { get; }
    }
}