using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using Maze.Server.MazeSockets.Internal;
using Maze.Sockets;
using Maze.Sockets.Internal;

namespace Maze.Server.MazeSockets
{
    public class MazeSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly MazeSocketOptions _options;

        public MazeSocketMiddleware(RequestDelegate next, IOptions<MazeSocketOptions> options)
        {
            _next = next ?? throw new ArgumentException(nameof(next));

            if (options == null)
                throw new ArgumentException(nameof(options));

            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Detect if an upgrade is available. If so, add a websocket upgrade.
            var upgradeFeature = context.Features.Get<IHttpUpgradeFeature>();
            if (upgradeFeature != null && IsMazeSocketRequest(context, upgradeFeature))
            {
                context.Features.Set<IMazeSocketFeature>(new MazeSocketUpgrader(context, upgradeFeature, _options));
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }

        private bool IsMazeSocketRequest(HttpContext context, IHttpUpgradeFeature upgradeFeature)
        {
            if (!upgradeFeature.IsUpgradableRequest)
                return false;

            if (!string.Equals(context.Request.Method, "GET", StringComparison.OrdinalIgnoreCase))
                return false;

            var headers = new List<KeyValuePair<string, string>>();
            foreach (var headerName in HandshakeHelpers.NeededHeaders)
            foreach (var value in context.Request.Headers.GetCommaSeparatedValues(headerName))
                headers.Add(new KeyValuePair<string, string>(headerName, value));

            return HandshakeHelpers.CheckSupportedRequest(headers);
        }

        private class MazeSocketUpgrader : IMazeSocketFeature
        {
            private readonly HttpContext _context;
            private readonly MazeSocketOptions _options;
            private readonly IHttpUpgradeFeature _upgradeFeature;

            public MazeSocketUpgrader(HttpContext context, IHttpUpgradeFeature upgradeFeature,
                MazeSocketOptions options)
            {
                _context = context;
                _upgradeFeature = upgradeFeature;
                _options = options;
            }

            public async Task<MazeSocket> AcceptAsync()
            {
                var key = string.Join(", ", _context.Request.Headers[MazeSocketHeaders.SecWebSocketKey]);

                var responseHeaders = HandshakeHelpers.GenerateResponseHeaders(key);
                foreach (var headerPair in responseHeaders)
                    _context.Response.Headers[headerPair.Key] = headerPair.Value;

                var stream = await _upgradeFeature.UpgradeAsync();
                await Task.Delay(500);

                string result;
                using (var streamReader = new StreamReader(stream))
                {
                    result = await streamReader.ReadLineAsync();
                }

                Debug.Print("Result Server: " + result);

                using (var streamWriter = new StreamWriter(stream))
                {
                    await streamWriter.WriteLineAsync("Hey other");
                }
                return new MazeSocket(stream, _options.KeepAliveInterval);
            }
        }
    }
}