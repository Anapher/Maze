using System;
using System.Linq;
using System.Threading.Tasks;
using Orcus.Modules.Api;
using Orcus.Modules.Api.Parameters;
using Orcus.Modules.Api.Routing;
using Orcus.Server.Connection.Utilities;
using Tasks.Infrastructure.Client.Library;
using Tasks.Infrastructure.Client.Storage;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Core.Dtos;
using Tasks.Infrastructure.Management;

namespace Tasks.Infrastructure.Client.Controllers
{
    [Route("v1/tasks")]
    public class TasksController : OrcusController
    {
        private readonly IClientTaskManager _clientTaskManager;

        public TasksController(IClientTaskManager clientTaskManager)
        {
            _clientTaskManager = clientTaskManager;
        }

        [OrcusPost]
        public async Task<IActionResult> CreateOrUpdateTask([FromServices] ITaskComponentResolver taskComponentResolver,
            [FromServices] IXmlSerializerCache serializerCache)
        {
            var orcusTask = new OrcusTaskReader(Request.Body, taskComponentResolver, serializerCache);
            var task = orcusTask.ReadTask();

            await _clientTaskManager.AddOrUpdateTask(task);
            return Ok();
        }

        [OrcusDelete("{taskId}")]
        public async Task<IActionResult> DeleteTask(Guid taskId)
        {
            await _clientTaskManager.RemoveTask(taskId);
            return Ok();
        }

        [OrcusGet("{taskId}/trigger")]
        public async Task<IActionResult> TriggerTask(Guid taskId, [FromQuery] string sessionKey, [FromServices] ITaskDirectory taskDirectory,
            [FromServices] IDatabaseTaskStorage databaseTaskStorage)
        {
            var task = (await taskDirectory.LoadTasks()).FirstOrDefault(x => x.Id == taskId);
            if (task == null)
                return NotFound();

            await _clientTaskManager.TriggerNow(task, SessionKey.FromHash(sessionKey), databaseTaskStorage);
            return Ok();
        }

        [OrcusPost("execute")]
        public async Task<IActionResult> ExecuteTask([FromServices] ITaskComponentResolver taskComponentResolver,
            [FromServices] IXmlSerializerCache serializerCache)
        {
            var orcusTask = new OrcusTaskReader(Request.Body, taskComponentResolver, serializerCache);
            var task = orcusTask.ReadTask();

            var memoryStorage = new MemoryTaskStorage();
            await _clientTaskManager.TriggerNow(task, SessionKey.Create("Execute"), memoryStorage);

            return Ok(new TaskSessionsInfo
            {
                Sessions = memoryStorage.Sessions.Select(x => new TaskSessionDto
                {
                    TaskReferenceId = x.TaskReferenceId,
                    TaskSessionId = x.TaskSessionId,
                    Description = x.Description,
                    CreatedOn = x.CreatedOn
                }).ToList(),
                Executions = memoryStorage.Executions.Select(x => new TaskExecutionDto
                {
                    TaskExecutionId = x.TaskExecutionId,
                    TaskReferenceId = x.TaskReferenceId,
                    TaskSessionId = x.TaskSessionId,
                    CreatedOn = x.CreatedOn
                }).ToList(),
                Results = memoryStorage.CommandResults.Select(x => new CommandResultDto
                {
                    CommandResultId = x.CommandResultId,
                    TaskExecutionId = x.TaskExecutionId,
                    CommandName = x.CommandName,
                    Result = x.Result,
                    Status = x.Status,
                    FinishedAt = x.FinishedAt
                }).ToList()
            });
        }
    }
}