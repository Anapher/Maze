using System.Threading.Tasks;

namespace Orcus.Server.Library.Tasks
{
    public abstract class TriggerContext
    {
        public abstract Task<TaskSessionTrigger> GetSession(string name);
        public abstract Task<bool> IsClientIncluded(int clientId);
        public abstract bool IsServerIncluded();
    }
}