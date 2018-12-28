using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Maze.Server.Library.Hubs
{
    /// <summary>
    ///     The SignalR Hub that manages the administration connections
    /// </summary>
    [Authorize]
    public class AdministrationHub : Hub
    {
    }
}