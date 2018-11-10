using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Orcus.Server.Library.Hubs
{
    [Authorize]
    public class AdministrationHub : Hub
    {
    }
}