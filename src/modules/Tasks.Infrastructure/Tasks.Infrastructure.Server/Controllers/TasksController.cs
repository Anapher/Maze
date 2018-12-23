using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Orcus.Server.Connection.Commanding;
using Orcus.Server.Connection.Utilities;
using Orcus.Server.Library.Controllers;
using Orcus.Server.Library.Hubs;
using Orcus.Server.Library.Utilities;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Core.Dtos;
using Tasks.Infrastructure.Management;
using Tasks.Infrastructure.Management.Data;
using Tasks.Infrastructure.Server.Business;
using Tasks.Infrastructure.Server.Business.TaskManager;
using Tasks.Infrastructure.Server.BusinessDataAccess;
using Tasks.Infrastructure.Server.Core;
using Tasks.Infrastructure.Server.Library;

namespace Tasks.Infrastructure.Server.Controllers
{
    [Route("Tasks.Infrastructure/v1/[controller]")]
    public class TasksController : BusinessController
    {
        [HttpPost]
        [Authorize("admin")]
        public async Task<IActionResult> CreateTask([FromServices] ITaskComponentResolver taskComponentResolver,
            [FromServices] IXmlSerializerCache serializerCache, [FromServices] ICreateTaskAction createTaskAction, [FromServices] IHubContext<AdministrationHub> hubContext)
        {
            var reader = new OrcusTaskReader(Request.Body, taskComponentResolver, serializerCache);
            var task = reader.ReadTask();

            await createTaskAction.BizActionAsync(task);

            return await BizActionStatus(createTaskAction, async () =>
            {
                await hubContext.Clients.All.SendAsync(HubEventNames.TaskCreated, task.Id);
                return Ok();
            });
        }

        [HttpDelete("{taskId}")]
        [Authorize("admin")]
        public async Task<IActionResult> RemoveTask(Guid taskId, [FromServices] IDeleteTaskAction deleteTaskAction, [FromServices] IHubContext<AdministrationHub> hubContext)
        {
            await deleteTaskAction.BizActionAsync(taskId);

            return await BizActionStatus(deleteTaskAction, async () =>
            {
                await hubContext.Clients.All.SendAsync(HubEventNames.TaskRemoved, taskId);
                return Ok();
            });
        }

        [HttpPut("{taskId}")]
        [Authorize("admin")]
        public async Task<IActionResult> UpdateTask(Guid taskId, [FromServices] ITaskComponentResolver taskComponentResolver,
            [FromServices] IXmlSerializerCache serializerCache, [FromServices] IUpdateTaskAction updateTaskAction,
            [FromServices] IHubContext<AdministrationHub> hubContext)
        {
            var orcusTask = new OrcusTaskReader(Request.Body, taskComponentResolver, serializerCache);
            var task = orcusTask.ReadTask();

            await updateTaskAction.BizActionAsync(task);

            return await BizActionStatus(updateTaskAction, async () =>
            {
                await hubContext.Clients.All.SendAsync(HubEventNames.TaskUpdated, taskId);
                return Ok();
            });
        }

        [HttpPost("{taskId}/enable")]
        [Authorize("admin")]
        public async Task<IActionResult> EnableTask(Guid taskId, [FromServices] IEnableTaskAction enableTaskAction, [FromServices] IHubContext<AdministrationHub> hubContext)
        {
            await enableTaskAction.BizActionAsync(taskId);

            return await BizActionStatus(enableTaskAction, async () =>
            {
                await hubContext.Clients.All.SendAsync(HubEventNames.TaskUpdated, taskId);
                return Ok();
            });
        }

        [HttpPost("{taskId}/disable")]
        [Authorize("admin")]
        public async Task<IActionResult> DisableTask(Guid taskId, [FromServices] IDisableTaskAction disableTaskAction, [FromServices] IHubContext<AdministrationHub> hubContext)
        {
            await disableTaskAction.BizActionAsync(taskId);

            return await BizActionStatus(disableTaskAction, async () =>
            {
                await hubContext.Clients.All.SendAsync(HubEventNames.TaskUpdated, taskId);
                return Ok();
            });
        }

        [HttpGet("{taskId}/trigger")]
        [Authorize("admin")]
        public async Task<IActionResult> TriggerTask(Guid taskId, [FromServices] ITriggerTaskAction triggerTaskAction)
        {
            await triggerTaskAction.BizActionAsync(taskId);
            return BizActionStatus(triggerTaskAction);
        }

        [HttpPost("execute")]
        [Authorize("admin")]
        public async Task<IActionResult> ExecuteTask([FromServices] ITaskComponentResolver taskComponentResolver,
            [FromServices] IXmlSerializerCache serializerCache,[FromServices] IExecuteTaskAction executeTaskAction)
        {
            var reader = new OrcusTaskReader(Request.Body, taskComponentResolver, serializerCache);
            var task = reader.ReadTask();

            var result = await executeTaskAction.BizActionAsync(task);
            return BizActionStatus(executeTaskAction, () => Ok(result));
        }

        [HttpGet("sync")]
        public async Task<IActionResult> GetSyncInfo([FromServices] IGetTaskSyncInfo getTaskSyncInfo)
        {
            if (User.IsAdministrator())
                return BadRequest();

            var tasks = await getTaskSyncInfo.BizActionAsync(User.GetClientId());
            return BizActionStatus(getTaskSyncInfo, () => Ok(tasks));
        }

        [HttpPost("status")]
        public async Task<IActionResult> UpdateMachineStatus([FromBody] IList<ClientTaskDto> tasks, [FromServices] ActiveTasksManager activeTasksManager)
        {
            if (User.IsAdministrator())
                return BadRequest();

            var clientId = User.GetClientId();

            var status = activeTasksManager.ActiveCommands.GetOrAdd(new TargetId(clientId), _ => new TasksMachineStatus());
            status.Tasks = tasks.ToImmutableList();

            return Ok();
        }

        [HttpGet]
        [Authorize("admin")]
        public async Task<IActionResult> GetTasks([FromServices] ITaskDirectory taskDirectory, [FromServices] IOrcusTaskManager taskManager, [FromServices] ITaskReferenceDbAccess dbAccess)
        {
            var tasks = await taskDirectory.LoadTasks();
            var taskInfos = await dbAccess.GetTasks();

            var infos = tasks.Select(x =>
            {
                var taskInfo = taskInfos[x.Id];
                var localActiveTask = taskManager.LocalActiveTasks.FirstOrDefault(y => y.OrcusTask.Id == x.Id);

                taskInfo.Name = x.Name;
                taskInfo.Commands = x.Commands.Count;
                taskInfo.IsCompletedOnServer = localActiveTask == null;
                taskInfo.NextExecution = localActiveTask?.NextExecution;

                return taskInfo;
            });

            return Ok(infos);
        }

        [HttpGet("{taskId}/info")]
        [Authorize("admin")]
        public async Task<IActionResult> GetTaskInfo(Guid taskId, [FromServices] ITaskDirectory taskDirectory, [FromServices] IOrcusTaskManager taskManager, [FromServices] ITaskReferenceDbAccess dbAccess)
        {
            var tasks = await taskDirectory.LoadTasks();
            var task = tasks.FirstOrDefault(x => x.Id == taskId);
            if (task == null)
                return NotFound();

            var localActiveTask = taskManager.LocalActiveTasks.FirstOrDefault(x => x.OrcusTask.Id == taskId);

            var taskInfo = await dbAccess.GetTaskInfo(taskId);
            taskInfo.Name = task.Name;
            taskInfo.Commands = task.Commands.Count;
            taskInfo.IsCompletedOnServer = localActiveTask == null;
            taskInfo.NextExecution = localActiveTask?.NextExecution;

            return Ok(taskInfo);
        }

        [HttpGet("{taskId}")]
        public async Task GetTask(Guid taskId, [FromServices] ITaskDirectory taskDirectory)
        {
            if (!User.IsAdministrator())
            {
                var action = HttpContext.RequestServices.GetRequiredService<ICreateTaskTransmissionAction>();
                await action.BizActionAsync(new TaskTransmission { TaskReferenceId = taskId, TargetId = User.GetClientId(), CreatedOn = DateTimeOffset.UtcNow});
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