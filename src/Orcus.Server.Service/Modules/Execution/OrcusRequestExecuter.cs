using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Orcus.Modules.Api;
using Orcus.Modules.Api.Request;
using Orcus.Modules.Api.Response;
using Orcus.Server.Service.Modules.Routing;

namespace Orcus.Server.Service.Modules.Execution
{
    public class OrcusRequestExecuter
    {
        private readonly IRouteResolver _routeResolver;
        private readonly IRouteCache _routeCache;
        private readonly ILogger<OrcusRequestExecuter> _logger;

        public OrcusRequestExecuter(IRouteResolver routeResolver, IRouteCache routeCache, ILogger<OrcusRequestExecuter> logger)
        {
            _routeResolver = routeResolver;
            _routeCache = routeCache;
            _logger = logger;
        }

        public async Task<OrcusResponse> Execute(OrcusRequest request)
        {
            var context = new RequestOrcusContext {Request = request};
            var result = _routeResolver.Resolve(context);
            if (!result.Success)
                return NotFound();

            var route = _routeCache.Routes[result.RouteDescription];
            result.
        }

        private OrcusResponse NotFound()
        {
            throw new NotImplementedException();
        }
    }

    public class RequestOrcusContext : OrcusContext
    {
        public override OrcusResponse Response { get; set; }
        public override object Caller { get; set; }
        public override OrcusRequest Request { get; set; }
        public override ConnectionInfo Connection { get; set; }
        public override IServiceProvider ServiceProvider { get; set; }
    }
}