using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Orcus.Server.Service.Modules.Routing
{
    public class Route
    {
        public Route(RouteDescription description, Type controllerType, MethodInfo routeMethod)
        {
            Description = description;
            ControllerType = controllerType;
            RouteMethod = routeMethod;
        }

        public RouteDescription Description { get; }
        public Type ControllerType { get; }
        public MethodInfo RouteMethod { get; }
    }
}