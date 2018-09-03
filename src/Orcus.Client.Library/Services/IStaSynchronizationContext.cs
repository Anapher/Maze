using System.Threading;

namespace Orcus.Client.Library.Services
{
    public interface IStaSynchronizationContext
    {
        SynchronizationContext Current { get; }
    }
}