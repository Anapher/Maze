using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orcus.Server.Connection.Utilities;
using Orcus.Server.Library.Controllers;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Server.Business;
using Tasks.Infrastructure.Server.Core;

namespace Tasks.Infrastructure.Server.Controllers
{
    [Route("Tasks.Infrastructure/v1/[controller]")]
    public class TasksController : BusinessController
    {
        [HttpPost, Authorize("admin")]
        public async Task<IActionResult> CreateTask([FromServices] ITaskComponentResolver taskComponentResolver,
            [FromServices] IXmlSerializerCache serializerCache, [FromServices] OrcusTaskManager taskManager)
        {
            var orcusTask = new OrcusTaskReader(Request.Body, taskComponentResolver, serializerCache);
            var task = orcusTask.ReadTask();

            await taskManager.AddTask(task);

            return Ok();
        }

        [HttpGet("sync")]
        public async Task<IActionResult> GetSyncInfo([FromServices] IGetTaskSyncInfo getTaskSyncInfo)
        {
            var tasks = await getTaskSyncInfo.BizActionAsync();
            return BizActionStatus(getTaskSyncInfo, () => Ok(tasks));
        }
    }
}
