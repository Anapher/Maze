using System.Threading.Tasks;
using Tasks.Infrastructure.Core.Data;

namespace Tasks.Infrastructure.Client.Library
{
    public abstract class TaskSessionTrigger
    {
        public abstract Task Invoke();

        public abstract TaskSession Info { get; }
    }
}