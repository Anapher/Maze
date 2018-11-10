using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Nito.AsyncEx;
using Tasks.Infrastructure.Server.Options;

namespace Tasks.Infrastructure.Server.BusinessDataAccess.Base
{
    public abstract class SqliteDbAccess
    {
        private readonly TasksOptions _options;

        protected SqliteDbAccess(IOptions<TasksOptions> options)
        {
            _options = options.Value;
        }

        protected AwaitableDisposable<SqliteConnection> OpenConnection() => new AwaitableDisposable<SqliteConnection>(GetConnectionInternal());

        private async Task<SqliteConnection> GetConnectionInternal()
        {
            var connection = new SqliteConnection(_options.ConnectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}