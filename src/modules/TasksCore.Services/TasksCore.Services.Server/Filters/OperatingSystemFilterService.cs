using Microsoft.EntityFrameworkCore;
using Orcus.Server.Connection.Tasks.Filter;
using Orcus.Server.Data.EfClasses;
using Orcus.Server.Data.EfCode;
using Orcus.Server.Library.Tasks;
using System.Threading.Tasks;

namespace TasksCore.Services.Server.Filters
{
   public class OperatingSystemFilterService : IFilterService<FilterInfo>
    {
        private readonly AppDbContext _context;

        public OperatingSystemFilterService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IncludeClient(FilterInfo filterInfo, int clientId)
        {
            var client = await _context.Set<Client>().FirstOrDefaultAsync(x => x.ClientId == clientId);
            //logic
            return true;
        }
    }
}
