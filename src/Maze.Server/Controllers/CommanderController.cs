using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Maze.Server.Connection;
using Maze.Server.Connection.Commanding;
using Maze.Server.Library.Controllers;
using Maze.Server.Library.Utilities;
using Maze.Server.Service;
using Maze.Server.Service.Commander;
using Maze.Server.Service.Extensions;
using Maze.Service.Commander;

namespace Maze.Server.Controllers
{
    public class CommanderController : BusinessController
    {
        //Path: v1/modules/Maze.RemoteDesktop/start
        [Route("v1/modules/{*path}"), Authorize("admin")]
        public async Task ExecuteCommand(string path, [FromServices] IMazeRequestExecuter requestExecuter,
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
                var mazeContext = new HttpMazeContextWrapper(HttpContext) {Request = {Path = "/" + path}};
                await requestExecuter.Execute(mazeContext, null /* TODO */);
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