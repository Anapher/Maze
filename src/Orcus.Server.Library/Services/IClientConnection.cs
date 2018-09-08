using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Orcus.Sockets;

namespace Orcus.Server.Library.Services
{
    public interface IClientConnection
    {
        int ClientId { get; }
        OrcusServer OrcusServer { get; }

        Task<HttpResponseMessage> SendRequest(HttpRequestMessage requestMessage, CancellationToken cancellationToken);
    }


    public interface IAdministrationConnection
    {
        int AccountId { get; }
        OrcusServer OrcusServer { get; }
    }
}