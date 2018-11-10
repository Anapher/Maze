using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Options;
using Tasks.Infrastructure.Server.BusinessDataAccess.Base;
using Tasks.Infrastructure.Management.Data;
using Tasks.Infrastructure.Server.Options;

namespace Tasks.Infrastructure.Server.BusinessDataAccess
{
    public interface IExecutionsDbAccess
    {
        Task CreateAsync(TaskExecution taskExecution);
    }

    public class ExecutionsDbAccess : SqliteDbAccess, IExecutionsDbAccess
    {
        public ExecutionsDbAccess(IOptions<TasksOptions> options) : base(options)
        {
        }

        public async Task CreateAsync(TaskExecution taskExecution)
        {
            using (var connection = await OpenConnection())
            {
                await connection.InsertAsync(taskExecution);
            }
        }
    }
}