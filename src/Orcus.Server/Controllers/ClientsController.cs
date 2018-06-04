using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orcus.Server.Authentication;
using Orcus.Server.Connection.Authentication.Client;
using Orcus.Server.Utilities;

namespace Orcus.Server.Controllers
{
    [Route("v1/[controller]")]
    public class ClientsController : Controller
    {
        [HttpPost("login")]
        [AllowAnonymous]
        [ValidateModelState]
        public async Task<IActionResult> Login([FromBody] ClientAuthenticationInfo authenticationInfo,
            [FromServices] IDefaultTokenProvider tokenProvider)
        {

        }
    }
}