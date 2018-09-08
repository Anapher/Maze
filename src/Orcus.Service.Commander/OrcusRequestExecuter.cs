using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Orcus.Modules.Api;
using Orcus.Modules.Api.Response;
using Orcus.Server.Connection;
using Orcus.Service.Commander.Routing;

namespace Orcus.Service.Commander
{
    /// <inheritdoc />
    public class OrcusRequestExecuter : IOrcusRequestExecuter
    {
        private readonly ILogger<OrcusRequestExecuter> _logger;
        private readonly IRouteCache _routeCache;
        private readonly IRouteResolver _routeResolver;

        public OrcusRequestExecuter(IRouteResolver routeResolver, IRouteCache routeCache,
            ILogger<OrcusRequestExecuter> logger)
        {
            _routeResolver = routeResolver;
            _routeCache = routeCache;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task Execute(OrcusContext context, IChannelServer channelServer)
        {
            _logger.LogDebug($"Resolve Orcus path {context.Request.Path}");
            var result = _routeResolver.Resolve(context);
            if (!result.Success)
            {
                _logger.LogDebug("Path not found");
                await WriteError(context, BusinessErrors.Commander.RouteNotFound(context.Request.Path),
                    StatusCodes.Status404NotFound);
                return;
            }

            _logger.LogDebug(
                $"Route resolved (package: {result.RouteDescription.PackageIdentity}). Get cached route info.");
            var route = _routeCache.Routes[result.RouteDescription];
            var actionContext = new DefaultActionContext(context, route, result.Parameters.ToImmutableDictionary());

            _logger.LogDebug($"Invoke method {route.RouteMethod}");

            IActionResult actionResult;
            try
            {
                switch (route.RouteType)
                {
                    case RouteType.Http:
                        actionResult = await route.ActionInvoker.Value.Invoke(actionContext);
                        break;
                    case RouteType.ChannelInit:
                        _logger.LogDebug("Create channel {channelName}", actionContext.Route.ControllerType.FullName);
                        var channel = await route.ActionInvoker.Value.InitializeChannel(actionContext, channelServer);

                        context.Response.Headers.Add(HeaderNames.Location, "ws://channels/" + channel.ChannelId);
                        actionResult = new StatusCodeResult(StatusCodes.Status201Created);
                        break;
                    case RouteType.Channel:
                        var channelId = int.Parse(actionContext.Context.Request.Headers["ChannelId"]);
                        _logger.LogDebug("Request channel with id {channelId}", channelId);

                        var foundChannel = channelServer.GetChannel(channelId);
                        actionResult =
                            await route.ActionInvoker.Value.InvokeChannel(actionContext, (OrcusChannel) foundChannel);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception e)
            {
                if (context.RequestAborted.IsCancellationRequested)
                    return;

                _logger.LogError(e,
                    $"Error occurred when invoking method {route.RouteMethod} of package {result.RouteDescription.PackageIdentity} (path: {context.Request.Path})");
                await WriteError(context,
                    BusinessErrors.Commander.ActionError(e.GetType().Name, route.RouteMethod.Name, e.Message),
                    StatusCodes.Status500InternalServerError);
                return;
            }

            if (context.RequestAborted.IsCancellationRequested)
                return;

            try
            {
                await actionResult.ExecuteResultAsync(actionContext);
            }
            catch (Exception e)
            {
                if (context.RequestAborted.IsCancellationRequested)
                    return;

                _logger.LogError(e,
                    $"Error occurred when executing action result {route.RouteMethod} of package {result.RouteDescription.PackageIdentity} (path: {context.Request.Path})");
                await WriteError(context,
                    BusinessErrors.Commander.ResultExecutionError(e.GetType().Name, actionResult?.GetType().Name,
                        e.Message), StatusCodes.Status500InternalServerError);
                return;
            }

            _logger.LogDebug("Request successfully executed.");
        }

        private Task WriteError(OrcusContext context, RestError error, int statusCode)
        {
            var actionContext = new DefaultActionContext(context, null, ImmutableDictionary<string, object>.Empty);
            return new ObjectResult(error) {StatusCode = statusCode}.ExecuteResultAsync(actionContext);
        }
    }
}