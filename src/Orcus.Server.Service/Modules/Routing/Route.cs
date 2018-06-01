using System;
using System.Reflection;

namespace Orcus.Server.Service.Modules.Routing
{
    public class Route
    {
        public Route(RouteDescription description, Type controllerType, MethodInfo routeMethod)
        {
            Description = description;
        }

        public RouteDescription Description { get; }
    }
}