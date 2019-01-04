using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CodeElements.BizRunner;
using CodeElements.BizRunner.Generic;
using Maze.Server.BusinessDataAccess.Accounts;
using Maze.Server.Connection.Accounts;
using Maze.Server.Data.EfClasses;
using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

namespace Maze.Server.BusinessLogic.Accounts
{
    public interface IUpdateAccountAction : IGenericActionWriteDbAsync<PasswordProvidingAccountDto, Account>
    {
    }

    public class NonValidatingAccountDto : PasswordProvidingAccountDto
    {
        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext) => Enumerable.Empty<ValidationResult>();
    }

    public class UpdateAccountAction : BusinessActionErrors, IUpdateAccountAction
    {
        private readonly IAccountDbAccess _dbAccess;

        public UpdateAccountAction(IAccountDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public async Task<Account> BizActionAsync(PasswordProvidingAccountDto inputData)
        {
            //we map to account dto because it doesn't validate the password and we want to allow null passwords
            if (ValidateModelFailed(Mapper.Map<AccountDto>(inputData)))
                return default;

            var account = await _dbAccess.FindAsync(inputData.AccountId);
            account.IsEnabled = inputData.IsEnabled;
            account.Username = inputData.Username;

            if (!string.IsNullOrEmpty(inputData.Password)) account.Password = BCrypt.Net.BCrypt.HashPassword(account.Password);

            return account;
        }
    }
}