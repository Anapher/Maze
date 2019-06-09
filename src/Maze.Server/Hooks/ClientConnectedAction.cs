using System.Threading.Tasks;
using AutoMapper;
using Maze.Server.Connection;
using Maze.Server.Connection.Clients;
using Maze.Server.Data.EfCode;
using Maze.Server.Library.Hubs;
using Maze.Server.Library.Interfaces;
using Maze.Server.Library.Services;
using Microsoft.AspNetCore.SignalR;

namespace Maze.Server.Hooks
{
    public class ClientConnectedAction : IClientConnectedAction
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<AdministrationHub> _hubContext;

        public ClientConnectedAction(AppDbContext context, IHubContext<AdministrationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task Execute(IClientConnection context)
        {
            var client = await _context.Clients.FindAsync(context.ClientId);
            var dto = Mapper.Map<ClientDto>(client);
            dto.IsSocketConnected = true;
            await _hubContext.Clients.All.SendAsync(HubEventNames.ClientConnected, dto);
        }
    }
}
