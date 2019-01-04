using System.Threading.Tasks;
using CodeElements.BizRunner;
using CodeElements.BizRunner.Generic;
using Maze.Server.BusinessDataAccess.Accounts;
using Maze.Server.Connection.Accounts;
using Maze.Server.Data.EfClasses;

namespace Maze.Server.BusinessLogic.Accounts
{
    public interface ICreateAccountAction : IGenericActionWriteDbAsync<PasswordProvidingAccountDto, Account> { }

    public class CreateAccountAction : BusinessActionErrors, ICreateAccountAction
    {
        private readonly IAccountDbAccess _dbAccess;

        public CreateAccountAction(IAccountDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public Task<Account> BizActionAsync(PasswordProvidingAccountDto inputData)
        {
            if (ValidateModelFailed(inputData))
                return Task.FromResult<Account>(null);

            var account = new Account
            {
                Username = inputData.Username, IsEnabled = inputData.IsEnabled, Password = BCrypt.Net.BCrypt.HashPassword(inputData.Password)
            };
            _dbAccess.AddAccount(account);

            return Task.FromResult(account);
        }
    }
}