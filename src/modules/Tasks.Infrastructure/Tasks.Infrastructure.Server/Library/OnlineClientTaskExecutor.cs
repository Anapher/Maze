using Orcus.Server.Library.Services;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Tasks.Infrastructure.Server.Library
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TCommandInfo"></typeparam>
    public abstract class OnlineClientTaskExecutor<TCommandInfo> : TaskExecutor
    {
        private readonly IConnectionManager _connectionManager;

        protected OnlineClientTaskExecutor(IConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public async Task<HttpResponseMessage> InvokeAsync(TCommandInfo commandInfo, TargetId targetId, TaskExecutionContext context, CancellationToken cancellationToken)
        {
            if (targetId.IsServer)
                return ServerNotSupported();

            if (!_connectionManager.ClientConnections.TryGetValue(targetId.ClientId, out var connection))
                return ClientNotConnected();

            return await InvokeAsync(commandInfo, connection, context, cancellationToken);
        }

        public abstract Task<HttpResponseMessage> InvokeAsync(TCommandInfo commandInfo, IClientConnection connection, TaskExecutionContext context, CancellationToken cancellationToken);
    }
}
