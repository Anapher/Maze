using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Orcus.Server.Data.EfClasses;
using Orcus.Server.Data.EfCode;

namespace Orcus.Server.BusinessDataAccess.Clients
{
    public interface IClientDbAccess
    {
        Task<Client> FindClientByHardwareId(string hardwareId);
    }

    public class ClientDbAccess : IClientDbAccess
    {
        private readonly AppDbContext _context;

        public ClientDbAccess(AppDbContext context)
        {
            _context = context;
        }

        public Task<Client> FindClientByHardwareId(string hardwareId)
        {
            return _context.Clients.FirstOrDefaultAsync(x => x.HardwareId == hardwareId);
        }
    }
}