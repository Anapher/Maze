using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orcus.Server.Connection;
using Orcus.Server.Connection.Commanding;
using Orcus.Server.Library.Controllers;
using Orcus.Server.Library.Utilities;
using Orcus.Server.Service;
using Orcus.Server.Service.Commander;
using Orcus.Server.Service.Extensions;
using Orcus.Service.Commander;

namespace Orcus.Server.Controllers
{
    public class CommanderController : BusinessController
    {
        //Path: v1/modules/Orcus.RemoteDesktop/start
        [Route("v1/modules/{*path}"), Authorize("admin")]
        public async Task ExecuteCommand(string path, [FromServices] IOrcusRequestExecuter requestExecuter,
            [FromServices] ICommandDistributer commandDistributer)
        {
            Request.Headers.TryGetValue("CommandTarget", out var targetHeader);

            CommandTargetCollection targets;
            try
            {
                targets = CommandTargetCollection.Parse(targetHeader.ToString());
            }
            catch (ArgumentException)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            if (targets.TargetsServer)
            {
                var orcusContext = new HttpOrcusContextWrapper(HttpContext) {Request = {Path = "/" + path}};
                await requestExecuter.Execute(orcusContext, null /* TODO */);
            }
            else
            {
                if (!targets.IsSingleClient(out var clientId))
                {
                    await RestError(BusinessErrors.Commander.SingleCommandTargetRequired).ExecuteResultAsync(ControllerContext);
                    return;
                }

                HttpResponseMessage response;
                try
                {
                    response = await commandDistributer.Execute(Request.ToHttpRequestMessage(path), clientId, User.GetAccountId(),
                        HttpContext.RequestAborted);
                }
                catch (ClientNotFoundException)
                {
                    await RestError(BusinessErrors.Commander.ClientNotFound).ExecuteResultAsync(ControllerContext);
                    return;
                }
                catch (Exception)
                {
                    await RestError(BusinessErrors.Commander.ClientNotFound).ExecuteResultAsync(ControllerContext);
                    return;
                }

                await response.CopyToHttpResponse(Response);
            }
        }
    }
}