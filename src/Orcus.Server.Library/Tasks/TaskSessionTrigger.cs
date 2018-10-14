using System.Threading.Tasks;
using Orcus.Server.Data.EfClasses.Tasks;

namespace Orcus.Server.Library.Tasks
{
    public abstract class TaskSessionTrigger
    {
        public abstract Task InvokeAll();
        public abstract Task InvokeClient(int clientId);
        public abstract Task InvokeServer();

        public abstract TaskSession Info { get; }
    }
}