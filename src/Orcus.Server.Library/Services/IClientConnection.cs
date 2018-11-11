using Orcus.Server.Library.Clients;
using Orcus.Sockets;

namespace Orcus.Server.Library.Services
{
    public interface IClientConnection : IRestClient
    {
        int ClientId { get; }
        OrcusServer OrcusServer { get; }
    }
}