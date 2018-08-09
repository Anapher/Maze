using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
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
            var socket = await connector.ConnectAsync(_options.KeepAliveInterval);
            var server = new OrcusServer(socket, _options.PackageBufferSize, _options.MaxHeaderSize);

            Listener = new ServerCommandListener(socket, server, lifetimeScope);
        }
    }
}