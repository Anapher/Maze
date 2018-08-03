using System.Reflection;
using Orcus.ModuleManagement.Loader;

namespace Orcus.Core.Modules
{
    public class PackageCarrier
    {
        public PackageCarrier(Assembly assembly, PackageLoadingContext context)
        {
            Assembly = assembly;
            Context = context;
        }

        public Assembly Assembly { get; }
        public PackageLoadingContext Context { get; }
    }
}