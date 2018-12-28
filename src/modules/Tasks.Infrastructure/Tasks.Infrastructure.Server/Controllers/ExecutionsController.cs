using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Maze.Server.Library.Controllers;
using Maze.Server.Library.Hubs;
using Maze.Server.Library.Utilities;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Core.Dtos;
using Tasks.Infrastructure.Server.Business;
using Tasks.Infrastructure.Server.Core;
using Tasks.Infrastructure.Server.Library;

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
                await _hubContext.Clients.All.SendAsync(HubEventNames.TaskExecutionCreated, taskExecutionDto);
                return Ok();
            });
        }

        [HttpPost("results")]
        public async Task<IActionResult> CreateCommandResult([FromBody] CommandResultDto commandResultDto,
            [FromServices] ICreateTaskCommandResultAction action, [FromServices] ActiveTasksManager activeTasksManager)
        {
            if (activeTasksManager.ActiveCommands.TryGetValue(new TargetId(User.GetClientId()), out var status))
                status.Processes.TryRemove(commandResultDto.CommandResultId, out _);

            await action.BizActionAsync(commandResultDto);
            return await BizActionStatus(action, async () =>
            {
                await _hubContext.Clients.All.SendAsync(HubEventNames.TaskCommandResultCreated, commandResultDto);
                return Ok();
            });
        }

        [HttpPost("process")]
        public async Task<IActionResult> CreateCommandProcess([FromBody] CommandProcessDto commandResultDto, [FromServices] ActiveTasksManager activeTasksManager)
        {
            var clientId = User.GetClientId();

            var status = activeTasksManager.ActiveCommands.GetOrAdd(new TargetId(clientId), _ => new TasksMachineStatus());
            status.Processes.AddOrUpdate(commandResultDto.CommandResultId, commandResultDto, (id, _) => commandResultDto);

            await _hubContext.Clients.All.SendAsync(HubEventNames.TaskCommandProcess, commandResultDto);
            return Ok();
        }
    }
}