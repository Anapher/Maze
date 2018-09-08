using System;
using System.Buffers;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Autofac;
using Orcus.Core.Commanding;
using Orcus.Server.Connection.Modules;
using Orcus.Sockets;
using Orcus.Sockets.Client;

namespace Orcus.Core.Connection
{
    public class ServerConnection
    {
        private readonly OrcusSocketOptions _options;

        public ServerConnection(OrcusRestClient restClient, PackagesLock packagesLock, OrcusSocketOptions options)
        {
            _options = options;
            RestClient = restClient;
            PackagesLock = packagesLock;
        }

        public OrcusRestClient RestClient { get; }
        public PackagesLock PackagesLock { get; }
        public ServerCommandListener Listener { get; private set; }

        public async Task InitializeWebSocket(ILifetimeScope lifetimeScope)
        {
            var builder = new UriBuilder(RestClient.BaseUri)
            {
                Path = "ws",
                Scheme = RestClient.BaseUri.Scheme == "https" ? "wss" : "ws"
            };

            var connector = new OrcusSocketConnector(builder.Uri)
            {
                AuthenticationHeaderValue = new AuthenticationHeaderValue("Bearer", RestClient.Jwt)
            };
            var dataStream = await connector.ConnectAsync();
            var webSocket = WebSocket.CreateClientWebSocket(dataStream, null, _options.PackageBufferSize, _options.PackageBufferSize,
                _options.KeepAliveInterval, true, WebSocket.CreateClientBuffer(_options.PackageBufferSize, _options.PackageBufferSize));

            var wrapper = new WebSocketWrapper(webSocket, _options.PackageBufferSize);
            var server = new OrcusServer(wrapper, _options.PackageBufferSize, _options.MaxHeaderSize, ArrayPool<byte>.Shared);

            Listener = new ServerCommandListener(connector, wrapper, server, lifetimeScope);
            await Listener.Listen();
        }
    }
}