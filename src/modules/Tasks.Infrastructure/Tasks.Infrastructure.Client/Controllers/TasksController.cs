using System;
using System.Threading.Tasks;
using Orcus.Modules.Api;
using Orcus.Modules.Api.Parameters;
using Orcus.Modules.Api.Routing;
using Orcus.Server.Connection.Utilities;
using Tasks.Infrastructure.Core;

namespace Tasks.Infrastructure.Client.Controllers
{
    [Route("tasks")]
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
        public async Task<IActionResult> TriggerTask(Guid taskId)
        {
            await _clientTaskManager.TriggerNow(taskId);
            return Ok();
        }
    }
}