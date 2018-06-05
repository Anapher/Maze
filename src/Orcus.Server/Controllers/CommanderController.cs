using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Orcus.Server.Controllers
{
    [Route("v1/pkg")]
    public class CommanderController : Controller
    {
        //Path: v1/modules/Orcus.RemoteDesktop/start
        //Target: Server
        [HttpPost("v1/modules/{packageId}/{*packagePath}")]
        public Task<IActionResult> ExecuteCommand(string packageId, string packagePath)
        {
            throw new NotImplementedException();
        }

        [HttpPost("v1/execute")]
        public Task<IActionResult> ExecuteCommand()
        {
            throw new NotImplementedException();
        }
    }
}