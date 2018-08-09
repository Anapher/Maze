using System;
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

        public int ClientId { get; }
        public OrcusSocket OrcusSocket { get; }
        public OrcusServer OrcusServer { get; }

        public void Dispose()
        {
            OrcusSocket?.Dispose();
            OrcusServer?.Dispose();
        }
    }
}