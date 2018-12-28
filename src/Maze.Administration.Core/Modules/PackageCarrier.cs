using System.Reflection;
using Maze.ModuleManagement.Loader;

namespace Maze.Administration.Core.Modules
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