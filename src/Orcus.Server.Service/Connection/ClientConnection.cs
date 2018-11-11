using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Orcus.Server.Library.Services;
using Orcus.Sockets;

namespace Orcus.Server.Service.Connection
{
    public class ClientConnection : IClientConnection, IDisposable
    {
        public ClientConnection(int clientId, WebSocketWrapper webSocket, OrcusServer orcusServer)
        {
            ClientId = clientId;
            WebSocketWrapper = webSocket;
            OrcusServer = orcusServer;
        }

        public void Dispose()
        {
            WebSocketWrapper?.Dispose();
            OrcusServer?.Dispose();
        }

        public int ClientId { get; }
        public WebSocketWrapper WebSocketWrapper { get; }
        public OrcusServer OrcusServer { get; }

        public Task BeginListen()
        {
            return WebSocketWrapper.ReceiveAsync();
        }

        public Task<HttpResponseMessage> SendMessage(HttpRequestMessage requestMessage, CancellationToken cancellationToken) =>
            OrcusServer.SendRequest(requestMessage, cancellationToken);
    }
}