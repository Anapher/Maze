using System;

namespace Orcus.Modules.Api.Routing
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class OrcusAttribute : Attribute, IRouteFragment, IActionMethodProvider
    {
        protected OrcusAttribute(string method) : this(method, null)
        {
        }

        protected OrcusAttribute(string method, string path)
        {
            Method = method;
            Path = path;
        }

        public string Method { get; }
        public string Path { get; }
    }
}