using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Options;
using Tasks.Infrastructure.Core.Dtos;
using Tasks.Infrastructure.Server.BusinessDataAccess.Base;
using Tasks.Infrastructure.Management.Data;
using Tasks.Infrastructure.Server.Dapper;
using Tasks.Infrastructure.Server.Options;

namespace Tasks.Infrastructure.Server.BusinessDataAccess
{
    public interface ITaskReferenceDbAccess
    {
        Task<TaskReference> FindAsync(Guid taskId);
        Task CreateAsync(TaskReference taskReference);
        Task DeleteAsync(Guid taskId);
        Task SetCompletionStatus(Guid taskId, bool status);
        Task SetTaskIsEnabled(Guid taskId, bool isEnabled);

        Task<IList<TaskSession>> GetSessions(Guid taskId);
        Task<IList<TaskExecution>> GetExecutions(Guid taskId);
        Task<IList<CommandResult>> GetCommandResults(Guid taskId);

        Task<IDictionary<Guid, TaskInfoDto>> GetTasks();
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
                    "SELECT TaskId, IsCompleted, AddedOn FROM TaskReference WHERE TaskId = @taskId", new {taskId});
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

        public async Task DeleteAsync(Guid taskId)
        {
            using (var connection = await OpenConnection())
            {
                await connection.ExecuteAsync(
                    "DELETE FROM CommandResult INNER JOIN TaskExecution ON CommandResult.TaskExecutionId = TaskExecution.TaskExecutionId WHERE TaskExecution.TaskReferenceId = @taskId",
                    new {taskId});
                await connection.ExecuteAsync("DELETE FROM TaskExecution WHERE TaskReferenceId = @taskId", new {taskId});
                await connection.ExecuteAsync("DELETE FROM TaskSession WHERE TaskReferenceId = @taskId", new {taskId});
                await connection.ExecuteAsync("DELETE FROM TaskTransmission WHERE TaskReferenceId = @taskId", new {taskId});
                await connection.ExecuteAsync("DELETE FROM TaskReference WHERE TaskReferenceId = @taskId", new {taskId});
            }
        }

        public async Task SetCompletionStatus(Guid taskId, bool status)
        {
            using (var connection = await OpenConnection())
            {
                await connection.ExecuteAsync("UPDATE TaskReference SET IsCompleted = @status WHERE TaskId = @taskId",
                    new {status, taskId});
            }
        }

        public async Task SetTaskIsEnabled(Guid taskId, bool isEnabled)
        {
            using (var connection = await OpenConnection())
            {
                await connection.ExecuteAsync("UPDATE TaskReference SET IsEnabled = @isEnabled WHERE TaskId = @taskId", new {isEnabled, taskId});
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
                    "SELECT TaskExecution.TaskExecutionId, TaskExecution.TaskSessionId, TaskExecution.TargetId, TaskExecution.CreatedOn FROM TaskExecution INNER JOIN TaskSession ON TaskSession.TaskSessionId = TaskExecution.TaskSessionId WHERE TaskSession.TaskReferenceId = @taskId", new { taskId });

                return executions.ToList();
            }
        }

        public async Task<IList<CommandResult>> GetCommandResults(Guid taskId)
        {
            using (var connection = await OpenConnection())
            {
                //No dapper mapping because of InvalidCaseException of Status
                //https://github.com/StackExchange/Dapper/issues/1001

                var reader = connection.ExecuteReader(
                    "SELECT CommandResult.CommandResultId, CommandResult.TaskExecutionId, CommandResult.CommandName, CommandResult.Result, CommandResult.Status, CommandResult.FinishedAt FROM CommandResult INNER JOIN TaskExecution ON TaskExecution.TaskExecutionId = CommandResult.TaskExecutionId INNER JOIN TaskSession ON TaskSession.TaskSessionId = TaskExecution.TaskSessionId WHERE TaskSession.TaskReferenceId = @taskId",
                    new {taskId});
                var result = new List<CommandResult>();

                using (reader)
                {
                    while (reader.Read())
                    {
                        result.Add(new CommandResult
                        {
                            CommandResultId = GuidTypeHandler.Parse((byte[]) reader.GetValue(0)),
                            TaskExecutionId = GuidTypeHandler.Parse((byte[])reader.GetValue(1)),
                            CommandName = reader.GetString(2),
                            Result = reader.GetString(3),
                            Status = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4),
                            FinishedAt = DateTimeOffset.FromUnixTimeMilliseconds(reader.GetInt64(5))
                        });
                    }
                }

                return result;
            }
        }

        public async Task<IDictionary<Guid, TaskInfoDto>> GetTasks()
        {
            using (var connection = await OpenConnection())
            {
                var tasks = (await connection.QueryAsync<TaskInfoDto>("SELECT TaskId AS Id, IsEnabled, AddedOn FROM TaskReference")).ToDictionary(x => x.Id, x => x);

                var executions = await connection.QueryAsync<TaskInfoDto>(
                    "SELECT TaskReferenceId AS `Id`, COUNT(*) AS TotalExecutions FROM TaskExecution GROUP BY TaskReferenceId");
                foreach (var taskInfoDto in executions)
                    tasks[taskInfoDto.Id].TotalExecutions = taskInfoDto.TotalExecutions;

                var sessions = await connection.QueryAsync<TaskSession>("SELECT TaskSessionId, TaskReferenceId FROM TaskSession");
                foreach (var taskSession in sessions)
                {
                    var task = tasks[taskSession.TaskReferenceId];
                    if (task.Sessions == null)
                        task.Sessions = new List<string>();

                    task.Sessions.Add(taskSession.TaskSessionId);
                }

                var lastExecutions =
                    await connection.QueryAsync<TaskInfoDto>("SELECT TaskReferenceId AS Id, MAX(CreatedOn) AS LastExecution FROM TaskExecution GROUP BY TaskReferenceId");
                foreach (var lastExecution in lastExecutions)
                    tasks[lastExecution.Id].LastExecution = lastExecution.LastExecution;

                return tasks;
            }
        }
    }
}