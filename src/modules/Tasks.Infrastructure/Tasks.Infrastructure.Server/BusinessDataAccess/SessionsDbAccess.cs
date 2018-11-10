using System;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Options;
using Tasks.Infrastructure.Server.BusinessDataAccess.Base;
using Tasks.Infrastructure.Management.Data;
using Tasks.Infrastructure.Server.Options;

namespace Tasks.Infrastructure.Server.BusinessDataAccess
{
    public interface ISessionsDbAccess
    {
        Task<TaskSession> FindAsync(string sessionId, Guid taskId);
        Task CreateAsync(TaskSession taskSession);
    }

    public class SessionsDbAccess : SqliteDbAccess, ISessionsDbAccess
    {
        public SessionsDbAccess(IOptions<TasksOptions> options) : base(options)
        {
        }

        public async Task<TaskSession> FindAsync(string sessionId, Guid taskId)
        {
            using (var connection = await OpenConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<TaskSession>(
                    "SELECT TaskSessionId, TaskReferenceId, Description, CreatedOn FROM TaskSession WHERE TaskSessionId = @sessionId AND TaskReferenceId = @taskId",
                    new {sessionId, taskId});
            }
        }

        public async Task CreateAsync(TaskSession taskSession)
        {
            using (var connection = await OpenConnection())
            {
                await connection.QueryAsync(
                    "INSERT INTO TaskSession (TaskSessionId, TaskReferenceId, Description, CreatedOn) VALUES (@TaskSessionId, @TaskReferenceId, @Description, @CreatedOn)",
                    taskSession);
            }
        }
    }
}