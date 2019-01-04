using System.Threading.Tasks;
using Maze.Server.Data.EfClasses;
using Maze.Server.Data.EfCode;
using Microsoft.EntityFrameworkCore;

namespace Maze.Server.BusinessDataAccess.Accounts
{
    public interface IAccountDbAccess
    {
        Task<Account> FindAccountByUsername(string username);
        void AddAccount(Account account);
        Task<Account> FindAsync(int accountId);
        void Delete(Account account);
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

        public void AddAccount(Account account)
        {
            _context.Accounts.Add(account);
        }

        public Task<Account> FindAsync(int accountId) => _context.Accounts.FindAsync(accountId);

        public void Delete(Account account)
        {
            _context.Accounts.Remove(account);
        }
    }
}