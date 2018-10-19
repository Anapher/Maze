using System;

namespace Tasks.Infrastructure.Core
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class OrcusCommandAttribute : Attribute
    {
        public string Name { get; }
        public string Modules { get; }

        public OrcusCommandAttribute(string name, string modules)
        {
            Name = name;
            Modules = modules;
        }
    }
}