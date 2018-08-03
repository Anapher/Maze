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

        /// <summary>
        ///     The root directory of the package
        /// </summary>
        public string PackageDirectory { get; }

        /// <summary>
        ///     The directory of the library (in the lib folder with the correct framework)
        /// </summary>
        public string LibraryDirectory { get; }

        /// <summary>
        ///     The selected framework version for this library
        /// </summary>
        public NuGetFramework Framework { get; }

        /// <summary>
        ///     True if the package is an Orcus module
        /// </summary>
        public bool IsOrcusModule { get; }
    }
}