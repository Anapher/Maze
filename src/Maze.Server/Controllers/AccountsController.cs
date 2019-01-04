using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CodeElements.BizRunner;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Maze.Server.Authentication;
using Maze.Server.BusinessLogic.Accounts;
using Maze.Server.BusinessLogic.Authentication;
using Maze.Server.Connection;
using Maze.Server.Connection.Accounts;
using Maze.Server.Connection.Authentication;
using Maze.Server.Data.EfClasses;
using Maze.Server.Data.EfCode;
using Maze.Server.Library.Controllers;
using Microsoft.EntityFrameworkCore;

namespace Maze.Server.Controllers
{
    [Route("v1/[controller]")]
    public class AccountsController : BusinessController
    {
        private readonly AppDbContext _context;

        public AccountsController(AppDbContext context)
        {
            _context = context;
        }

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

        [HttpGet]
        [Authorize("admin")]
        public IEnumerable<AccountDto> GetAccounts()
        {
            return _context.Accounts.ProjectTo<AccountDto>();
        }

        [HttpPost]
        [Authorize("admin")]
        public async Task<IActionResult> PostAccount([FromBody] PasswordProvidingAccountDto account, [FromServices] ICreateAccountAction action)
        {
            Account result;
            try
            {
                result = await action.ToRunner(_context).ExecuteAsync(account);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (action.HasErrors)
                    return BizActionStatus(action);

                if (AccountExists(account.Username))
                    return RestError(BusinessErrors.Account.UsernameAlreadyExists);
                else throw;
            }

            return BizActionStatus(action, () => CreatedAtAction("GetAccount", new {id = account.AccountId}, Mapper.Map<AccountDto>(result)));
        }

        [HttpGet("{id}")]
        [Authorize("admin")]
        public async Task<IActionResult> GetAccount([FromRoute] int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
                return RestError(BusinessErrors.Account.NotFound);

            return Ok(Mapper.Map<AccountDto>(account));
        }

        [HttpPut("{id}")]
        [Authorize("admin")]
        public async Task<IActionResult> PutAccount([FromRoute] int id, [FromBody] NonValidatingAccountDto accountDto, [FromServices] IUpdateAccountAction action)
        {
            if (accountDto.AccountId != id)
                return BadRequest();

            await action.ToRunner(_context).ExecuteAsync(accountDto);
            return BizActionStatus(action, Ok);
        }

        [HttpDelete("{id}")]
        [Authorize("admin")]
        public async Task<IActionResult> DeleteAccount([FromRoute] int id, [FromServices] IDeleteAccountAction action)
        {
            await action.ToRunner(_context).ExecuteAsync(id);
            return BizActionStatus(action, Ok);
        }

        private bool AccountExists(string username)
        {
            return _context.Accounts.Any(e => e.Username == username);
        }
    }
}