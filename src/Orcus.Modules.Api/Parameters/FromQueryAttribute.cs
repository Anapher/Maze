using System;

namespace Orcus.Modules.Api.Parameters
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class FromQueryAttribute : Attribute
    {
        public string Name { get; set; }
    }
}