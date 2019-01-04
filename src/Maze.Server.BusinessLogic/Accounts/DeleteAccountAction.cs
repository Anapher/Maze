using System.Threading.Tasks;
using CodeElements.BizRunner;
using CodeElements.BizRunner.Generic;
using Maze.Server.BusinessDataAccess.Accounts;
using Maze.Server.Connection;
using Maze.Server.Data.EfClasses;

namespace Maze.Server.BusinessLogic.Accounts
{
    public interface IDeleteAccountAction : IGenericActionWriteDbAsync<int, Account>
    {
    }

    public class DeleteAccountAction : BusinessActionErrors, IDeleteAccountAction
    {
        private readonly IAccountDbAccess _dbAccess;

        public DeleteAccountAction(IAccountDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public async Task<Account> BizActionAsync(int inputData)
        {
            var account = await _dbAccess.FindAsync(inputData);
            if (account == null)
                return ReturnError<Account>(BusinessErrors.Account.NotFound);

            _dbAccess.Delete(account);
            return account;
        }
    }
}