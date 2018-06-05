using System.Collections.Immutable;

namespace Orcus.Server.Service.Modules.Routing
{
    public interface IRouteCache
    {
        IImmutableDictionary<RouteDescription, Route> Routes { get; set; }
    }
}