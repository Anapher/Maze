using System;
using System.Net.Http;
using System.Threading.Tasks;
using Orcus.Server.Library.Services;
using Orcus.Sockets;

namespace Orcus.Server.Service.Connection
{
    public class ClientConnection : IClientConnection, IDisposable
    {
        public ClientConnection(int clientId, OrcusSocket orcusSocket, OrcusServer orcusServer)
        {
            ClientId = clientId;
            OrcusSocket = orcusSocket;
            OrcusServer = orcusServer;
        }

        public void Dispose()
        {
            OrcusSocket?.Dispose();
            OrcusServer?.Dispose();
        }

        public int ClientId { get; }
        public OrcusSocket OrcusSocket { get; }
        public OrcusServer OrcusServer { get; }

        public Task BeginListen()
        {
            return OrcusSocket.ReceiveAsync();
        }

        public Task<HttpResponseMessage> SendRequest(HttpRequestMessage requestMessage) =>
            OrcusServer.SendRequest(requestMessage);
    }
}