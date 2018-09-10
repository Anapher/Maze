using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orcus.Modules.Api;
using Orcus.Modules.Api.Routing;

namespace TaskManager.Client.Controllers
{
    public class TaskManagerController : OrcusController
    {
        [OrcusGet("processes")]
        public async Task<IActionResult> QueryProcesses()
        {

        }
    }
}
