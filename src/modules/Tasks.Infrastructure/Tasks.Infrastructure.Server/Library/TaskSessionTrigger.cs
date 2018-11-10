using System.Threading.Tasks;
using Tasks.Infrastructure.Management.Data;

namespace Tasks.Infrastructure.Server.Library
{
    public abstract class TaskSessionTrigger
    {
        public abstract Task Invoke();
        public abstract Task InvokeClient(int clientId);
        public abstract Task InvokeServer();

        public abstract TaskSession Info { get; }
    }
}