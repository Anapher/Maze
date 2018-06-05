using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using Orcus.Server.OrcusSockets.Internal;

namespace Orcus.Server.OrcusSockets
{
    public class OrcusSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private OrcusSocketOptions _options;

        public OrcusSocketMiddleware(RequestDelegate next, IOptions<OrcusSocketOptions> options)
        {
            _next = next ?? throw new ArgumentException(nameof(next));

            if (options == null)
                throw new ArgumentException(nameof(options));

            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Detect if an opaque upgrade is available. If so, add a websocket upgrade.
            var upgradeFeature = context.Features.Get<IHttpUpgradeFeature>();
            if (upgradeFeature != null && IsOrcusSocketRequest(context, upgradeFeature))
            {
                string key = string.Join(", ", context.Request.Headers[Headers.SecWebSocketKey]);

                var responseHeaders = HandshakeHelpers.GenerateResponseHeaders(key);
                foreach (var headerPair in responseHeaders)
                    context.Response.Headers[headerPair.Key] = headerPair.Value;

                var stream = await upgradeFeature.UpgradeAsync();

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
    }
}