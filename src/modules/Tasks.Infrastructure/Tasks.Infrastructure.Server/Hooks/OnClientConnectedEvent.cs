using System.Threading.Tasks;
using Orcus.Server.Library.Interfaces;
using Orcus.Server.Library.Services;
using Tasks.Infrastructure.Server.Core;

namespace Tasks.Infrastructure.Server.Hooks
{
    public class OnClientConnectedEvent : IClientConnectedAction
    {
        private readonly ITasksConnectionManager _connectionManager;

        public OnClientConnectedEvent(ITasksConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public Task Execute(IClientConnection context)
        {
            var clientInfo = new ConnectedClientInfo(context);
            _connectionManager.Clients.TryAdd(context.ClientId, clientInfo);

            return Task.CompletedTask;
        }
    }
}