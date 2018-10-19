using System;

namespace Orcus.Server.Connection.Tasks
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