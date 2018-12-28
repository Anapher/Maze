using System;
using System.Buffers;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Maze.ModuleManagement;
using Maze.Server.Library.Interfaces;
using Maze.Server.Library.Services;
using Maze.Server.Library.Utilities;
using Maze.Server.Service.Connection;
using Maze.Sockets;
using Maze.Utilities;

namespace Maze.Server.Middleware
{
    public class MazeSocketManagerMiddleware
    {
        private readonly IConnectionManager _connectionManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly RequestDelegate _next;
        private readonly MazeSocketOptions _options;

        public MazeSocketManagerMiddleware(RequestDelegate next, IConnectionManager connectionManager,
            IOptions<MazeSocketOptions> options, IServiceProvider serviceProvider)
        {
            _next = next;
            _connectionManager = connectionManager;
            _serviceProvider = serviceProvider;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await _next(context);
                return;
            }

            if (!context.User.Identity.IsAuthenticated)
            {
                context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return;
            }

            var socket = await context.WebSockets.AcceptWebSocketAsync();
            var wrapper = new WebSocketWrapper(socket, _options.PackageBufferSize);
            var server = new MazeServer(wrapper, _options.PackageBufferSize, _options.MaxHeaderSize, ArrayPool<byte>.Shared);

            if (context.User.IsAdministrator())
            {
                var accountId = context.User.GetAccountId();
                var connection = new AdministrationConnection(accountId, wrapper, server);

                _connectionManager.AdministrationConnections.TryAdd(accountId, connection);
                try
                {
                    await connection.BeginListen();
                }
                finally
                {
                    _connectionManager.AdministrationConnections.TryRemove(accountId, out _);
                }
            }
            else
            {
                var clientId = context.User.GetClientId();
                var connection = new ClientConnection(clientId, wrapper, server);
                _connectionManager.ClientConnections.TryAdd(clientId, connection);

                _serviceProvider.Execute<IClientConnectedAction, IClientConnection>(connection).Forget();

                try
                {
                    await connection.BeginListen();
                }
                finally
                {
                    _connectionManager.ClientConnections.TryRemove(clientId, out _);
                }

                await _serviceProvider.Execute<IClientDisconnectedAction, int>(connection.ClientId);
            }
        }
    }
}