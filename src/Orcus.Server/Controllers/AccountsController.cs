using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orcus.Server.Authentication;
using Orcus.Server.BusinessLogic.Authentication;
using Orcus.Server.Connection.Authentication;
using Orcus.Server.ControllersBase;

namespace Orcus.Server.Controllers
{
    [Route("v1/[controller]")]
    public class AccountsController : BusinessController
    {
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginInfo loginInfo,
            [FromServices] IAuthenticateAdministrationAction authenticateAdministrationAction,
            [FromServices] ITokenProvider tokenProvider)
        {
            var account = await authenticateAdministrationAction.BizActionAsync(loginInfo);
            return BizActionStatus(authenticateAdministrationAction, () =>
            {
                var token = tokenProvider.GetAccountToken(account);
                return Ok(tokenProvider.TokenToString(token));
            });
        }
    }
}