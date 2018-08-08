using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Orcus.Server.Service;
using Orcus.Server.Service.Commander;
using Orcus.Server.Service.Extensions;
using Orcus.Service.Commander;

namespace Orcus.Server.Controllers
{
    public class CommanderController : Controller
    {
        //Path: v1/modules/Orcus.RemoteDesktop/start
        //Target: Server
        [Route("v1/modules/{*path}")]
        public async Task ExecuteCommand(string path, [FromServices] IOrcusRequestExecuter requestExecuter)
        {
            var orcusContext = new HttpOrcusContextWrapper(HttpContext) {Request = {Path = "/" + path}};
            await requestExecuter.Execute(orcusContext);
        }

        //[HttpPost("v1/execute")]
        //public Task<IActionResult> ExecuteCommand()
        //{
        //    _commandDistributer.Execute(Request.ToHttpRequestMessage())
        //}
    }
}