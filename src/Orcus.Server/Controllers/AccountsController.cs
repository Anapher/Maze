using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orcus.Server.Authentication;
using Orcus.Server.Connection;
using Orcus.Server.Connection.Authentication;
using Orcus.Server.ControllersBase;
using Orcus.Server.Data.EfCode;
using Orcus.Server.Utilities;

namespace Orcus.Server.Controllers
{
    [Route("v1/[controller]")]
    public class AccountsController : BusinessController
    {
        private readonly AppDbContext _context;

        public AccountsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login"), AllowAnonymous, ValidateModelState]
        public async Task<IActionResult> Login([FromBody] LoginInfo loginInfo, [FromServices] IDefaultTokenProvider tokenProvider)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(x => x.Username == loginInfo.Username);
            if (account == null)
                return RestError(BusinessErrors.Account.UsernameNotFound);

            if (!account.IsEnabled)
                return RestError(BusinessErrors.Account.AccountDisabled);

            if (!BCrypt.Net.BCrypt.Verify(loginInfo.Password, account.Password))
                return RestError(BusinessErrors.Account.InvalidPassword);

            var token = tokenProvider.GetAccountToken(account);
            return Ok(tokenProvider.TokenToString(token));
        }
    }
}