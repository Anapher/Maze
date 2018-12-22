using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Orcus.Server.Library.Controllers;
using Orcus.Server.Library.Hubs;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Core.Dtos;
using Tasks.Infrastructure.Server.Business;

namespace Tasks.Infrastructure.Server.Controllers
{
    [Route("Tasks.Infrastructure/v1/[controller]")]
    public class SessionsController : BusinessController
    {
        private readonly IHubContext<AdministrationHub> _hubContext;

        public SessionsController(IHubContext<AdministrationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSession([FromBody] TaskSessionDto taskSessionDto, [FromServices] ICreateTaskSessionAction action)
        {
            await action.BizActionAsync(taskSessionDto);

            return await BizActionStatus(action, async () =>
            {
                await _hubContext.Clients.All.SendAsync(HubEventNames.TaskSessionCreated, taskSessionDto);
                return Ok();
            });
        }
    }
}