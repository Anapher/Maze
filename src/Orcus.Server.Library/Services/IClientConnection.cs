using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Orcus.Server.Library.Services
{
    public interface IClientConnection
    {
        int ClientId { get; }

        Task<HttpResponseMessage> SendRequest(HttpRequestMessage requestMessage, CancellationToken cancellationToken);
    }


    public interface IAdministrationConnection
    {
    }
}