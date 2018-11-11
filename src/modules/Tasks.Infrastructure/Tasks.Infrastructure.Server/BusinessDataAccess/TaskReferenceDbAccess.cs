using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Options;
using Tasks.Infrastructure.Server.BusinessDataAccess.Base;
using Tasks.Infrastructure.Management.Data;
using Tasks.Infrastructure.Server.Options;

namespace Tasks.Infrastructure.Server.BusinessDataAccess
{
    public interface ITaskReferenceDbAccess
    {
        Task<TaskReference> FindAsync(Guid taskId);
        Task CreateAsync(TaskReference taskReference);
        Task Delete(Guid taskId);
        Task SetCompletionStatus(Guid taskId, bool completionStatus);

        Task<IList<TaskSession>> GetSessions(Guid taskId);
        Task<IList<TaskExecution>> GetExecutions(Guid taskId);
        Task<IList<CommandResult>> GetCommandResults(Guid taskId);
    }

    public class TaskReferenceDbAccess : SqliteDbAccess, ITaskReferenceDbAccess
    {
        public TaskReferenceDbAccess(IOptions<TasksOptions> options) : base(options)
        {
        }

        public async Task<TaskReference> FindAsync(Guid taskId)
        {
            using (var connection = await OpenConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<TaskReference>(
                    "SELECT TaskId, IsCompleted FROM TaskReference WHERE TaskId = @taskId", new {taskId});
                //return await connection.GetAsync<TaskReference>(taskId);
            }
        }

        public async Task CreateAsync(TaskReference taskReference)
        {
            using (var connection = await OpenConnection())
            {
                await connection.InsertAsync(taskReference);
            }
        }

        public async Task Delete(Guid taskId)
        {
            using (var connection = await OpenConnection())
            {
                await connection.DeleteAsync(new TaskReference {TaskId = taskId});
            }
        }

        public async Task SetCompletionStatus(Guid taskId, bool completionStatus)
        {
            return;
            using (var connection = await OpenConnection())
            {
                await connection.QueryAsync("UPDATE TaskReference SET IsCompleted = @status WHERE TaskId = @taskId",
                    new {status = completionStatus, taskId});
            }
        }

        public async Task<IList<TaskSession>> GetSessions(Guid taskId)
        {
            using (var connection = await OpenConnection())
            {
                var sessions = await connection.QueryAsync<TaskSession>(
                    "SELECT TaskSessionId, Description, CreatedOn FROM TaskSession WHERE TaskReferenceId = @taskId", new {taskId});

                var list = sessions.ToList();
                foreach (var taskSession in list)
                    taskSession.TaskReferenceId = taskId;

                return list;
            }
        }

        public async Task<IList<TaskExecution>> GetExecutions(Guid taskId)
        {
            using (var connection = await OpenConnection())
            {
                var executions = await connection.QueryAsync<TaskExecution>(
                    "SELECT TaskExecution.TaskExecutionId, TaskExecution.TaskSessionId, TaskExecution.TargetId, TaskExecution.CreatedOn FROM TaskExecution INNER JOIN TaskSession ON TaskSession.TaskSessionId = TaskExecution.TaskExecutionId WHERE TaskSession.TaskReferenceId = @taskId", new { taskId });

                return executions.ToList();
            }
        }

        public async Task<IList<CommandResult>> GetCommandResults(Guid taskId)
        {
            using (var connection = await OpenConnection())
            {
                var executions = await connection.QueryAsync<CommandResult>(
                    "SELECT CommandResult.CommandResultId, CommandResult.TaskExecutionId, CommandResult.TargetId, CommandResult.CommandName, CommandResult.Result, CommandResult.Status, FinishedAt.Status FROM CommandResult INNER JOIN TaskExecution ON TaskExecution.TaskExecutionId = CommandResult.TaskExecutionId INNER JOIN TaskSession ON TaskSession.TaskSessionId = TaskExecution.TaskExecutionId WHERE TaskSession.TaskReferenceId = @taskId",
                    new {taskId});

                return executions.ToList();
            }
        }
    }
}