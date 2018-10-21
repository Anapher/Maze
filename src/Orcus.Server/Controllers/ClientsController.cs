using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using CodeElements.BizRunner;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orcus.Server.Authentication;
using Orcus.Server.BusinessLogic.Authentication;
using Orcus.Server.Connection.Authentication.Client;
using Orcus.Server.Connection.Clients;
using Orcus.Server.Data.EfCode;
using Orcus.Server.Library.Controllers;
using Orcus.Server.Library.Services;
using Orcus.Server.Service.Modules;

namespace Orcus.Server.Controllers
{
    [Route("v1/[controller]")]
    public class ClientsController : BusinessController
    {
        private readonly AppDbContext _context;

        public ClientsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] ClientAuthenticationDto authenticationDto,
            [FromServices] IAuthenticateClientAction authenticateClientAction,
            [FromServices] ITokenProvider tokenProvider,
            [FromServices] IModulePackageManager modulePackageManager)
        {
            var client = await authenticateClientAction.ToRunner(_context).ExecuteAsync(new ClientAuthenticationContext
            {
                Dto = authenticationDto,
                IpAddress = HttpContext.Connection.RemoteIpAddress
            });

            return await BizActionStatus(authenticateClientAction, async () =>
            {
                var token = tokenProvider.GetClientToken(client);
                return Ok(new ClientAuthenticationResponse
                {
                    Jwt = tokenProvider.TokenToString(token),
                    Modules = await modulePackageManager.GetPackagesLock(authenticationDto.Framework)
                });
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromServices] IConnectionManager connectionManager)
        {
            var clients = await _context.Clients.ProjectTo<ClientDto>().ToListAsync();
            foreach (var clientDto in clients)
                clientDto.IsSocketConnected = connectionManager.ClientConnections.ContainsKey(clientDto.ClientId);

            return Ok(clients);
        }
    }
}