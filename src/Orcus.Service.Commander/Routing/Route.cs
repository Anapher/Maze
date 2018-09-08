using System;
using System.Reflection;
using System.Threading;
using Orcus.Service.Commander.Commanding;

namespace Orcus.Service.Commander.Routing
{
    public class Route
    {
        public Route(RouteDescription description, Type controllerType, MethodInfo routeMethod, RouteType routeType)
        {
            Description = description;
            ControllerType = controllerType;
            RouteMethod = routeMethod;
            RouteType = routeType;

            ActionInvoker = new Lazy<ActionInvoker>(() => new ActionInvoker(controllerType, routeMethod),
                LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public Lazy<ActionInvoker> ActionInvoker { get; }
        public RouteDescription Description { get; }
        public Type ControllerType { get; }
        public MethodInfo RouteMethod { get; }
        public RouteType RouteType { get; }
    }

    public enum RouteType
    {
        Http,
        ChannelInit,
        Channel
    }
}