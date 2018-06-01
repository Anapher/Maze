using System;

namespace Orcus.Modules.Api.Routing
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class OrcusMethodAttribute : Attribute, IRouteFragment, IActionMethodProvider
    {
        protected OrcusMethodAttribute(string method) : this(method, null)
        {
        }

        protected OrcusMethodAttribute(string method, string path)
        {
            Method = method;
            Path = path;
        }

        public string Method { get; }
        public string Path { get; }
    }
}