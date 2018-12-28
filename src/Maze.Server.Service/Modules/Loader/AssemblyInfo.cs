using System.Reflection;
using System.Runtime.Loader;

namespace Maze.Server.Service.Modules.Loader
{
    public class AssemblyInfo
    {
        private readonly object _assemblyLock = new object();
        private Assembly _assembly;

        public string Path { get; }

        public AssemblyInfo(string path)
        {
            Path = path;
        }

        public Assembly LoadAssembly(AssemblyLoadContext assemblyLoadContext)
        {
            if (_assembly != null)
                return _assembly;

            lock (_assemblyLock)
            {
                if (_assembly == null)
                {
                    _assembly = assemblyLoadContext.LoadFromAssemblyPath(Path);
                }

                return _assembly;
            }
        }
    }
}