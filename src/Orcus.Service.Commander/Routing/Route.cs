using System;
using System.Reflection;
using System.Threading;
using Orcus.Server.Service.Commanding;

namespace Orcus.Server.Service.Modules.Routing
{
    public class Route
    {
        public Route(RouteDescription description, Type controllerType, MethodInfo routeMethod)
        {
            Description = description;
            ControllerType = controllerType;
            RouteMethod = routeMethod;

            ActionInvoker = new Lazy<ActionInvoker>(() => new ActionInvoker(controllerType, routeMethod),
                LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public Lazy<ActionInvoker> ActionInvoker { get; }
        public RouteDescription Description { get; }
        public Type ControllerType { get; }
        public MethodInfo RouteMethod { get; }
    }
}