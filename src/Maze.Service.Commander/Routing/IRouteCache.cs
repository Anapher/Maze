using System.Collections.Immutable;

namespace Maze.Service.Commander.Routing
{
    public interface IRouteCache
    {
        IImmutableDictionary<RouteDescription, Route> Routes { get; set; }
    }
}