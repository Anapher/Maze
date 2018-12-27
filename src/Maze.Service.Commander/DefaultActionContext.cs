using System.Collections.Generic;
using Orcus.Modules.Api;
using Orcus.Service.Commander.Routing;

namespace Orcus.Service.Commander
{
    public class DefaultActionContext : ActionContext
    {
        public DefaultActionContext(OrcusContext context, Route route, IReadOnlyDictionary<string, object> routeData)
        {
            Context = context;
            Route = route;
            RouteData = routeData;
        }

        public override OrcusContext Context { get; }
        public Route Route { get; }
        public override IReadOnlyDictionary<string, object> RouteData { get; }
    }
}