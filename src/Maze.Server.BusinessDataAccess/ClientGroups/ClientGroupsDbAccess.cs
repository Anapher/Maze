using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Maze.Server.Data.EfClasses;
using Maze.Server.Data.EfCode;

namespace Maze.Server.BusinessDataAccess.ClientGroups
{
    public interface IClientGroupsDbAccess
    {
        void AddClientGroup(ClientGroup clientGroup);
        Task<ClientGroup> FindAsync(int clientGroupId);
        void Remove(ClientGroup clientGroup);
    }

    public class ClientGroupsDbAccess : IClientGroupsDbAccess
    {
        private readonly AppDbContext _dbContext;

        public ClientGroupsDbAccess(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<ClientGroup> FindAsync(int clientGroupId) =>
            _dbContext.Groups.Include(x => x.ClientGroupMemberships).FirstOrDefaultAsync(x => x.ClientGroupId == clientGroupId);

        public void Remove(ClientGroup clientGroup)
        {
            _dbContext.Groups.Remove(clientGroup);
        }

        public void AddClientGroup(ClientGroup clientGroup)
        {
            _dbContext.Add(clientGroup);
        }
    }
}