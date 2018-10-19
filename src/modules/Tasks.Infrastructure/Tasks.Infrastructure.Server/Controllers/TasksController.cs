using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Orcus.Server.Connection.Utilities;
using Tasks.Infrastructure.Core;

namespace Tasks.Infrastructure.Server.Controllers
{
    [Route("v1/[controller]")]
    public class TasksController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromServices] ITaskComponentResolver taskComponentResolver,
            [FromServices] IXmlSerializerCache serializerCache, [FromServices] OrcusTaskManager taskManager)
        {
            var orcusTask = new OrcusTaskReader(Request.Body, taskComponentResolver, serializerCache);
            var task = orcusTask.ReadTask();

            await taskManager.AddTask(task);

            return Ok();
        }

        [HttpGet("sync")]
        public async Task GetSync()
        {

        }
    }
}
