using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Orcus.ModuleManagement.Loader;

namespace Orcus.Core.Modules
{
    public interface IPackageLoader
    {
        PackageCarrier Load(PackageLoadingContext loadingContext);
    }

    public class PackageLoader : IPackageLoader
    {
        public PackageCarrier Load(PackageLoadingContext loadingContext)
        {
            var libFolder = new DirectoryInfo(loadingContext.LibraryDirectory);
            var dlls = libFolder.GetFiles("*.dll");

            if (!dlls.Any())
                throw new InvalidOperationException(
                    $"Cannot load package {loadingContext.Package} ({loadingContext.PackageDirectory}) because there are no dlls");

            if (dlls.Length > 1)
                throw new NotImplementedException();

            var file = dlls.Single();
            return new PackageCarrier(Assembly.LoadFile(file.FullName), loadingContext);
        }
    }
}