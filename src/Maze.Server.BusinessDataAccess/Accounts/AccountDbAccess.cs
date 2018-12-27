using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Orcus.Server.Data.EfClasses;
using Orcus.Server.Data.EfCode;

namespace Orcus.Server.BusinessDataAccess.Accounts
{
    public interface IAccountDbAccess
    {
        Task<Account> FindAccountByUsername(string username);
    }

    public class AccountDbAccess : IAccountDbAccess
    {
        private readonly AppDbContext _context;

        public AccountDbAccess(AppDbContext context)
        {
            _context = context;
        }

        public Task<Account> FindAccountByUsername(string username)
        {
            return _context.Accounts.FirstOrDefaultAsync(x => x.Username == username);
        }
    }
}