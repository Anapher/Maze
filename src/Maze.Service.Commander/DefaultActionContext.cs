using System.Collections.Generic;
using Maze.Modules.Api;
using Maze.Service.Commander.Routing;

namespace Maze.Service.Commander
{
    public class DefaultActionContext : ActionContext
    {
        public DefaultActionContext(MazeContext context, Route route, IReadOnlyDictionary<string, object> routeData)
        {
            Context = context;
            Route = route;
            RouteData = routeData;
        }

        public override MazeContext Context { get; }
        public Route Route { get; }
        public override IReadOnlyDictionary<string, object> RouteData { get; }
    }
}