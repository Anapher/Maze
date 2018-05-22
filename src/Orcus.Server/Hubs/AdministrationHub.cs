using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Orcus.Server.Hubs
{
    [Authorize]
    public class AdministrationHub : Hub
    {
    }
}