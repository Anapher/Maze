using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Maze.ModuleManagement;
using Maze.ModuleManagement.Loader;

namespace Maze.Core.Modules
{
    public interface IPackageLoader
    {
        IEnumerable<PackageCarrier> Load(PackageLoadingContext loadingContext);
    }

    public class PackageLoader : IPackageLoader
    {
        public IEnumerable<PackageCarrier> Load(PackageLoadingContext loadingContext)
        {
            var libFolder = new DirectoryInfo(loadingContext.LibraryDirectory);
            var files = LibraryLocator.GetFilesToLoad(libFolder, loadingContext);

            return files.Select(x => new PackageCarrier(Assembly.LoadFile(x.FullName), loadingContext));
        }
    }
}