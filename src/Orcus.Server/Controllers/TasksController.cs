using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Orcus.Server.Connection.Tasks;
using Orcus.Server.Connection.Utilities;

namespace Orcus.Server.Controllers
{
    [Route("v1/[controller]")]
    public class TasksController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromServices] ITaskComponentResolver taskComponentResolver,
            [FromServices] IXmlSerializerCache serializerCache)
        {
            var orcusTask = new OrcusTaskReader(Request.Body, taskComponentResolver, serializerCache);
            orcusTask.ReadTask();


            return Ok();
        }
    }
}
