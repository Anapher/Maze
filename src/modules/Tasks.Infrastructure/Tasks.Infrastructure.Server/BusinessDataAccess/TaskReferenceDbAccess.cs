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
    public interface ITaskReferenceDbAccess
    {
        Task<TaskReference> FindAsync(Guid taskId);
        Task CreateAsync(TaskReference taskReference);
        Task Delete(Guid taskId);
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
                return await connection.GetAsync<TaskReference>(taskId);
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
    }
}