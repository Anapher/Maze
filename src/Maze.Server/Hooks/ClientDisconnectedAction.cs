using Maze.Server.Connection;
using Maze.Server.Library.Hubs;
using Maze.Server.Library.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Maze.Server.Hooks
{
    public class ClientDisconnectedAction : IClientDisconnectedAction
    {
        private readonly IHubContext<AdministrationHub> _hubContext;

        public ClientDisconnectedAction(IHubContext<AdministrationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task Execute(int clientId)
        {
            return _hubContext.Clients.All.SendAsync(HubEventNames.ClientDisconnected, clientId);
        }
    }
}
