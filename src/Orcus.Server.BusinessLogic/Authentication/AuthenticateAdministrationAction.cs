using System.Threading.Tasks;
using CodeElements.BizRunner.Generic;
using Microsoft.Extensions.Logging;
using Orcus.Server.BusinessDataAccess.Accounts;
using Orcus.Server.Connection;
using Orcus.Server.Connection.Authentication;
using Orcus.Server.Data.EfClasses;

namespace Orcus.Server.BusinessLogic.Authentication
{
    public interface IAuthenticateAdministrationAction : IGenericActionAsync<LoginInfo, Account> { }

    public class AuthenticateAdministrationAction : LoggingBusinessActionErrors, IAuthenticateAdministrationAction
    {
        private readonly IAccountDbAccess _dbAccess;

        public AuthenticateAdministrationAction(IAccountDbAccess dbAccess,
            ILogger<AuthenticateAdministrationAction> logger) : base(logger)
        {
            _dbAccess = dbAccess;
        }

        public async Task<Account> BizActionAsync(LoginInfo inputData)
        {
            if (ValidateModelFailed(inputData))
                return default;

            Logger.LogDebug("Try to find user account with username {username}", inputData.Username);
            var account = await _dbAccess.FindAccountByUsername(inputData.Username);

            if (account == null)
                return ReturnError<Account>(BusinessErrors.Account.UsernameNotFound);

            Logger.LogTrace("User account found! {@account}", account);

            if (!account.IsEnabled)
                return ReturnError<Account>(BusinessErrors.Account.AccountDisabled);

            if (!BCrypt.Net.BCrypt.Verify(inputData.Password, account.Password))
                return ReturnError<Account>(BusinessErrors.Account.InvalidPassword);

            return account;
        }
    }
}