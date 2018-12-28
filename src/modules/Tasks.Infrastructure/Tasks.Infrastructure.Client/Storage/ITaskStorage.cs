using System.Threading.Tasks;
using Tasks.Infrastructure.Client.Library;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Management.Data;

namespace Tasks.Infrastructure.Client.Storage
{
    public interface ITaskStorage
    {
        Task<TaskSession> OpenSession(SessionKey sessionKey, MazeTask mazeTask, string description);
        Task StartExecution(MazeTask mazeTask, TaskSession taskSession, TaskExecution taskExecution);
        Task AppendCommandResult(MazeTask mazeTask, CommandResult commandResult);
        Task MarkTaskFinished(MazeTask mazeTask);
        Task<bool> CheckTaskFinished(MazeTask mazeTask);
    }
}