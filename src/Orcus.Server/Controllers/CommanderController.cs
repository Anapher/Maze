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
        public async Task<IActionResult> ExecuteCommand(string packageId, string packagePath)
        {

        }

        [HttpPost("v1/execute")]
        public async Task<IActionResult> ExecuteCommand()
        {

        }
    }
}