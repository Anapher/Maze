using System.Buffers;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orcus.Server.Authentication;
using Orcus.Server.Library.Services;
using Orcus.Server.Service.Connection;
using Orcus.Sockets;

namespace Orcus.Server.Middleware
{
    public class OrcusSocketManagerMiddleware
    {
        private readonly IConnectionManager _connectionManager;
        private readonly ILoggerFactory _loggerFactory;
        private readonly RequestDelegate _next;
        private readonly OrcusSocketOptions _options;

        public OrcusSocketManagerMiddleware(RequestDelegate next, IConnectionManager connectionManager,
            IOptions<OrcusSocketOptions> options, ILoggerFactory loggerFactory)
        {
            _next = next;
            _connectionManager = connectionManager;
            _options = options.Value;
            _loggerFactory = loggerFactory;
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
            var server = new OrcusServer(wrapper, _options.PackageBufferSize, _options.MaxHeaderSize, ArrayPool<byte>.Shared);

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

                try
                {
                    await connection.BeginListen();
                }
                finally
                {
                    _connectionManager.ClientConnections.TryRemove(clientId, out _);
                }
            }
        }
    }
}