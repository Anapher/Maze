using System;
using System.Threading.Tasks;

namespace Tasks.Infrastructure.Server.Filter
{
    public interface IClientFilter
    {
        int? Cost { get; }
        Task<bool> Invoke(int clientId, IServiceProvider serviceProvider);
    }
}