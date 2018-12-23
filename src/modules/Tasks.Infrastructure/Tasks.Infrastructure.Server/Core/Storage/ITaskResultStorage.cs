using System.Threading.Tasks;
using Tasks.Infrastructure.Core.Dtos;
using Tasks.Infrastructure.Management.Data;

namespace Tasks.Infrastructure.Server.Core.Storage
{
    public interface ITaskResultStorage
    {
        Task<TaskSession> CreateTaskSession(TaskSessionDto taskSession);
        Task<bool> CreateTaskExecution(TaskExecutionDto taskExecution);
        Task CreateCommandResult(CommandResultDto commandResult);
    }
}