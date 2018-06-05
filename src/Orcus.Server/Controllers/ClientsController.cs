using System.Threading.Tasks;
using CodeElements.BizRunner;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Frameworks;
using Orcus.Server.Authentication;
using Orcus.Server.BusinessLogic.Authentication;
using Orcus.Server.Connection.Authentication.Client;
using Orcus.Server.ControllersBase;
using Orcus.Server.Data.EfCode;
using Orcus.Server.Service.Modules;
using Orcus.Server.Utilities;

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
        [ValidateModelState]
        public async Task<IActionResult> Login([FromBody] ClientAuthenticationDto authenticationDto,
            [FromServices] IAuthenticateClientAction authenticateClientAction,
            [FromServices] IDefaultTokenProvider tokenProvider,
            [FromServices] IModulePackageManager modulePackageManager)
        {
            var client = await authenticateClientAction.ToRunner(_context).ExecuteAsync(new ClientAuthenticationInfo
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
                    Modules = await modulePackageManager.GetPackagesLock(new NuGetFramework(
                        FrameworkConstants.CommonFrameworks.OrcusClient10.Framework,
                        authenticationDto.ClientVersion.Version))
                });
            });
        }
    }
}