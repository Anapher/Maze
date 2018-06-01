using NuGet.Frameworks;
using NuGet.Packaging.Core;

namespace Orcus.ModuleManagement.Loader
{
    public class PackageLoadingContext
    {
        public PackageLoadingContext(PackageIdentity package, string packageDirectory, string libraryDirectory,
            NuGetFramework framework, bool isOrcusModule)
        {
            Package = package;
            PackageDirectory = packageDirectory;
            LibraryDirectory = libraryDirectory;
            Framework = framework;
            IsOrcusModule = isOrcusModule;
        }

        public PackageIdentity Package { get; }
        public string PackageDirectory { get; }
        public string LibraryDirectory { get; }
        public NuGetFramework Framework { get; }
        public bool IsOrcusModule { get; }
    }
}