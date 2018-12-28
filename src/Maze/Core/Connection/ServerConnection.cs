using System;
using System.Buffers;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Autofac;
using Maze.Client.Library.Services;
using Maze.Core.Commanding;
using Maze.Server.Connection.Modules;
using Maze.Sockets;
using Maze.Sockets.Client;

namespace Maze.Core.Connection
{
    public class ServerConnection : IServerConnection
    {
        private readonly MazeSocketOptions _options;

        public ServerConnection(MazeRestClient restClient, PackagesLock packagesLock, MazeSocketOptions options)
        {
            _options = options;
            RestClient = restClient;
            PackagesLock = packagesLock;
        }

        public IMazeRestClient RestClient { get; }
        public PackagesLock PackagesLock { get; }
        public ServerCommandListener Listener { get; private set; }

        public async Task InitializeWebSocket(ILifetimeScope lifetimeScope)
        {
            var builder = new UriBuilder(RestClient.BaseUri)
            {
                Path = "ws",
                Scheme = RestClient.BaseUri.Scheme == "https" ? "wss" : "ws"
            };

            var connector = new MazeSocketConnector(builder.Uri)
            {
                AuthenticationHeaderValue = new AuthenticationHeaderValue("Bearer", RestClient.Jwt)
            };
            var dataStream = await connector.ConnectAsync();
            var webSocket = WebSocket.CreateClientWebSocket(dataStream, null, _options.PackageBufferSize, _options.PackageBufferSize,
                _options.KeepAliveInterval, true, WebSocket.CreateClientBuffer(_options.PackageBufferSize, _options.PackageBufferSize));

            var wrapper = new WebSocketWrapper(webSocket, _options.PackageBufferSize);
            var server = new MazeServer(wrapper, _options.PackageBufferSize, _options.MaxHeaderSize, ArrayPool<byte>.Shared);

            Listener = new ServerCommandListener(connector, wrapper, server, lifetimeScope);
            await Listener.Listen();
        }
    }
}