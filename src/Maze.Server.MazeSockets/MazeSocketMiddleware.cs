using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using Orcus.Server.OrcusSockets.Internal;
using Orcus.Sockets;
using Orcus.Sockets.Internal;

namespace Orcus.Server.OrcusSockets
{
    public class OrcusSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly OrcusSocketOptions _options;

        public OrcusSocketMiddleware(RequestDelegate next, IOptions<OrcusSocketOptions> options)
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
            if (upgradeFeature != null && IsOrcusSocketRequest(context, upgradeFeature))
            {
                context.Features.Set<IOrcusSocketFeature>(new OrcusSocketUpgrader(context, upgradeFeature, _options));
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }

        private bool IsOrcusSocketRequest(HttpContext context, IHttpUpgradeFeature upgradeFeature)
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

        private class OrcusSocketUpgrader : IOrcusSocketFeature
        {
            private readonly HttpContext _context;
            private readonly OrcusSocketOptions _options;
            private readonly IHttpUpgradeFeature _upgradeFeature;

            public OrcusSocketUpgrader(HttpContext context, IHttpUpgradeFeature upgradeFeature,
                OrcusSocketOptions options)
            {
                _context = context;
                _upgradeFeature = upgradeFeature;
                _options = options;
            }

            public async Task<OrcusSocket> AcceptAsync()
            {
                var key = string.Join(", ", _context.Request.Headers[OrcusSocketHeaders.SecWebSocketKey]);

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
                return new OrcusSocket(stream, _options.KeepAliveInterval);
            }
        }
    }
}