using System;
using System.Reflection;

namespace Orcus.Server.Service.Modules.Loader
{
    public class ModuleLoadingException : Exception
    {
        public ModuleLoadingException(string message, Assembly assembly) : base(
            $"An error occurred when trying to load assembly {assembly.FullName}: {message}")
        {
        }
    }
}