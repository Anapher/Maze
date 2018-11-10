using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Orcus.Server.Library.Controllers;
using Orcus.Server.Library.Hubs;
using Orcus.Server.Library.Utilities;
using Tasks.Infrastructure.Core.Dtos;
using Tasks.Infrastructure.Server.Business;
using Tasks.Infrastructure.Server.Core;

namespace Tasks.Infrastructure.Server.Controllers
{
    [Route("Tasks.Infrastructure/v1/[controller]")]
    public class ExecutionsController : BusinessController
    {
        private readonly IHubContext<AdministrationHub> _hubContext;

        public ExecutionsController(IHubContext<AdministrationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<IActionResult> CreateExecution([FromBody] TaskExecutionDto taskExecutionDto, [FromServices] ICreateTaskExecutionAction action)
        {
            taskExecutionDto.TargetId = User.GetClientId();

            await action.BizActionAsync(taskExecutionDto);

            return await BizActionStatus(action, async () =>
            {
                await _hubContext.Clients.All.SendAsync("TaskExecutionCreated", taskExecutionDto);
                return Ok();
            });
        }

        [HttpPost("results")]
        public async Task<IActionResult> CreateCommandResult([FromBody] CommandResultDto commandResultDto,
            [FromServices] ICreateTaskCommandResultAction action, [FromServices] ITasksConnectionManager connectionManager)
        {
            if (connectionManager.Clients.TryGetValue(User.GetClientId(), out var connectedClient))
                connectedClient.CommandProcesses.TryRemove(commandResultDto.CommandResultId, out _);

            await action.BizActionAsync(commandResultDto);

            return await BizActionStatus(action, async () =>
            {
                await _hubContext.Clients.All.SendAsync("TaskCommandResultCreated", commandResultDto);
                return Ok();
            });
        }

        [HttpPost("process")]
        public async Task<IActionResult> CreateCommandProcess([FromBody] CommandProcessDto commandResultDto, [FromServices] ITasksConnectionManager connectionManager)
        {
            var clientId = User.GetClientId();
            if (connectionManager.Clients.TryGetValue(clientId, out var connectedClient))
            {
                commandResultDto.TargetId = clientId;
                connectedClient.CommandProcesses.AddOrUpdate(commandResultDto.CommandResultId, commandResultDto, (id, _) => commandResultDto);
            }

            await _hubContext.Clients.All.SendAsync("TaskCommandProcess", commandResultDto);
            return Ok();
        }
    }
}