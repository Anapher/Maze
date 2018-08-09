using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orcus.Server.Authentication;
using Orcus.Server.Library.Services;
using Orcus.Server.OrcusSockets;
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
            var socketFeature = context.Features.Get<IOrcusSocketFeature>();
            if (socketFeature == null)
            {
                await _next(context);
                return;
            }

            if (!context.User.Identity.IsAuthenticated)
            {
                context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return;
            }

            var socket = await socketFeature.AcceptAsync();
            var server = new OrcusServer(socket, _options.PackageBufferSize, _options.MaxHeaderSize);

            if (context.User.IsAdministrator())
            {
            }
            else
            {
                var clientId = context.User.GetClientId();
                var connection = new ClientConnection(clientId, socket, server);
                _connectionManager.ClientConnections.TryAdd(clientId, connection);
            }
        }
    }
}