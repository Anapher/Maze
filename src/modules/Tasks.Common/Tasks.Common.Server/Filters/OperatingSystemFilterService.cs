using Microsoft.EntityFrameworkCore;
using Orcus.Server.Data.EfClasses;
using Orcus.Server.Data.EfCode;
using System.Threading.Tasks;
using Tasks.Infrastructure.Server.Library;
using TasksCore.Services.Shared.Filters;

namespace TasksCore.Services.Server.Filters
{
    public class OperatingSystemFilterService : IFilterService<OperatingSystemFilterInfo>
    {
        private readonly AppDbContext _context;

        public OperatingSystemFilterService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IncludeClient(OperatingSystemFilterInfo filterInfo, int clientId)
        {
            var client = await _context.Set<Client>().FirstOrDefaultAsync(x => x.ClientId == clientId);
            //logic
            return true;
        }
    }
}
