using System;
using System.Threading.Tasks;
using Orcus.Server.Library.Services;
using Orcus.Sockets;

namespace Orcus.Server.Service.Connection
{
    public class AdministrationConnection : IAdministrationConnection, IDisposable
    {
        public AdministrationConnection(int accountId, WebSocketWrapper webSocket, OrcusServer orcusServer)
        {
            AccountId = accountId;
            WebSocket = webSocket;
            OrcusServer = orcusServer;
        }

        public void Dispose()
        {
            WebSocket?.Dispose();
            OrcusServer?.Dispose();
        }

        public int AccountId { get; }
        public WebSocketWrapper WebSocket { get; }
        public OrcusServer OrcusServer { get; }

        public Task BeginListen()
        {
            return WebSocket.ReceiveAsync();
        }
    }
}