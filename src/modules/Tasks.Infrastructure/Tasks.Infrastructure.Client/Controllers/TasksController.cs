using System;
using System.Linq;
using System.Threading.Tasks;
using Maze.Modules.Api;
using Maze.Modules.Api.Parameters;
using Maze.Modules.Api.Routing;
using Maze.Server.Connection.Utilities;
using Tasks.Infrastructure.Client.Library;
using Tasks.Infrastructure.Client.Storage;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Core.Dtos;
using Tasks.Infrastructure.Management;

namespace Tasks.Infrastructure.Client.Controllers
{
    [Route("v1/tasks")]
    public class TasksController : MazeController
    {
        private readonly IClientTaskManager _clientTaskManager;

        public TasksController(IClientTaskManager clientTaskManager)
        {
            _clientTaskManager = clientTaskManager;
        }

        [MazePost]
        public async Task<IActionResult> CreateOrUpdateTask([FromServices] ITaskComponentResolver taskComponentResolver,
            [FromServices] IXmlSerializerCache serializerCache)
        {
            var mazeTask = new MazeTaskReader(Request.Body, taskComponentResolver, serializerCache);
            var task = mazeTask.ReadTask();

            await _clientTaskManager.AddOrUpdateTask(task);
            return Ok();
        }

        [MazeDelete("{taskId}")]
        public async Task<IActionResult> DeleteTask(Guid taskId)
        {
            await _clientTaskManager.RemoveTask(taskId);
            return Ok();
        }

        [MazeGet("{taskId}/trigger")]
        public async Task<IActionResult> TriggerTask(Guid taskId, [FromQuery] string sessionKey, [FromServices] ITaskDirectory taskDirectory,
            [FromServices] IDatabaseTaskStorage databaseTaskStorage)
        {
            var task = (await taskDirectory.LoadTasks()).FirstOrDefault(x => x.Id == taskId);
            if (task == null)
                return NotFound();

            await _clientTaskManager.TriggerNow(task, SessionKey.FromHash(sessionKey), databaseTaskStorage);
            return Ok();
        }

        [MazePost("execute")]
        public async Task<IActionResult> ExecuteTask([FromServices] ITaskComponentResolver taskComponentResolver,
            [FromServices] IXmlSerializerCache serializerCache)
        {
            var mazeTask = new MazeTaskReader(Request.Body, taskComponentResolver, serializerCache);
            var task = mazeTask.ReadTask();

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