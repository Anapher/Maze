using System.Linq;
using System.Web;
using Orcus.Modules.Api;
using Orcus.Server.Service.Modules.Routing.Trie;

namespace Orcus.Server.Service.Modules.Routing
{
    public class RouteResolver : IRouteResolver
    {
        private readonly IRouteResolverTrie _routeResolverTrie;

        public RouteResolver(IRouteResolverTrie routeResolverTrie)
        {
            _routeResolverTrie = routeResolverTrie;
        }

        public ResolveResult Resolve(OrcusContext context)
        {
            var pathDecoded = HttpUtility.UrlDecode(context.Request.Path);

            var results = _routeResolverTrie.GetMatches(context.Request.Method, pathDecoded);
            if (!results.Any())
                return new ResolveResult(false);

            var matchResult = results.OrderByDescending(x => x).First();
            return new ResolveResult
            {
                RouteDescription = matchResult.RouteDescription,
                Parameters = matchResult.Parameters
            };
        }
    }
}