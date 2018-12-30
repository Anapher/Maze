using System.Threading.Tasks;
using Maze.Server.Data.EfClasses;
using Maze.Server.Data.EfCode;
using Microsoft.EntityFrameworkCore;

namespace Maze.Server.BusinessDataAccess.ClientConfigurations
{
    public interface IClientConfigurationDbAccess
    {
        Task<ClientConfiguration> FindAsync(int? groupId);
        void Add(ClientConfiguration clientConfiguration);
        void Remove(ClientConfiguration clientConfiguration);
    }

    public class ClientConfigurationDbAccess : IClientConfigurationDbAccess
    {
        private readonly AppDbContext _context;

        public ClientConfigurationDbAccess(AppDbContext context)
        {
            _context = context;
        }

        public Task<ClientConfiguration> FindAsync(int? groupId)
        {
            return _context.Set<ClientConfiguration>().FirstOrDefaultAsync(x => x.ClientGroupId == groupId);
        }

        public void Add(ClientConfiguration clientConfiguration)
        {
            _context.Add(clientConfiguration);
        }

        public void Remove(ClientConfiguration clientConfiguration)
        {
            _context.Remove(clientConfiguration);
        }
    }
}