using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Orcus.Sockets.Client;

namespace Orcus.Core.Commanding
{
    public class WebSocketConnectionInfo : ConnectionInfo
    {
        public WebSocketConnectionInfo(OrcusSocketConnector connector)
        {
            Id = "ServerConnection";
            if (connector.TcpClient.Client.RemoteEndPoint is IPEndPoint remoteEndpoint)
            {
                RemoteIpAddress = remoteEndpoint.Address;
                RemotePort = remoteEndpoint.Port;
            }

            if (connector.TcpClient.Client.LocalEndPoint is IPEndPoint localEndpoint)
            {
                LocalIpAddress = localEndpoint.Address;
                LocalPort = localEndpoint.Port;
            }
        }

        public override Task<X509Certificate2> GetClientCertificateAsync(CancellationToken cancellationToken = new CancellationToken()) => throw new NotSupportedException();

        public override string Id { get; set; }
        public override IPAddress RemoteIpAddress { get; set; }
        public override int RemotePort { get; set; }
        public override IPAddress LocalIpAddress { get; set; }
        public override int LocalPort { get; set; }
        public override X509Certificate2 ClientCertificate { get; set; }
    }
}