using System;

namespace Orcus.Modules.Api.Routing
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RouteAttribute : Attribute, IRouteFragment
    {
        public RouteAttribute(string path)
        {
            Path = path;
        }

        public string Path { get; }
    }
}