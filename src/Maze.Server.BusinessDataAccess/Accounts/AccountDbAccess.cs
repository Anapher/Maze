using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Maze.Server.Data.EfClasses;
using Maze.Server.Data.EfCode;

namespace Maze.Server.BusinessDataAccess.Accounts
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