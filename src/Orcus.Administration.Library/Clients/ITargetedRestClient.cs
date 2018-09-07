using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Orcus.Administration.Library.Clients
{
    public interface ITargetedRestClient : IRestClient
    {
        Task<TChannel> OpenChannel<TChannel>(HttpRequestMessage message, CancellationToken cancellationToken);
    }
}