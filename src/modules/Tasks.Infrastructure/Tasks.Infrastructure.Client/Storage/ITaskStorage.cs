using System.Threading.Tasks;
using Tasks.Infrastructure.Client.Library;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Management.Data;

namespace Tasks.Infrastructure.Client.Storage
{
    public interface ITaskStorage
    {
        Task<TaskSession> OpenSession(SessionKey sessionKey, OrcusTask orcusTask, string description);
        Task StartExecution(OrcusTask orcusTask, TaskSession taskSession, TaskExecution taskExecution);
        Task AppendCommandResult(OrcusTask orcusTask, CommandResult commandResult);
        Task MarkTaskFinished(OrcusTask orcusTask);
        Task<bool> CheckTaskFinished(OrcusTask orcusTask);
    }
}