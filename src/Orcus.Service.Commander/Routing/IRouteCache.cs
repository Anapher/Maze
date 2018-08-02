using System.Collections.Immutable;

namespace Orcus.Service.Commander.Routing
{
    public interface IRouteCache
    {
        IImmutableDictionary<RouteDescription, Route> Routes { get; set; }
    }
}