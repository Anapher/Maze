using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Options;
using Tasks.Infrastructure.Server.BusinessDataAccess.Base;
using Tasks.Infrastructure.Management.Data;
using Tasks.Infrastructure.Server.Options;

namespace Tasks.Infrastructure.Server.BusinessDataAccess
{
    public interface ICommandResultsDbAccess
    {
        Task CreateAsync(CommandResult commandResult);
    }

    public class CommandResultsDbAccess : SqliteDbAccess, ICommandResultsDbAccess
    {
        public CommandResultsDbAccess(IOptions<TasksOptions> options) : base(options)
        {
        }

        public async Task CreateAsync(CommandResult commandResult)
        {
            using (var connection = await OpenConnection())
            {
                await connection.InsertAsync(commandResult);
            }
        }
    }
}