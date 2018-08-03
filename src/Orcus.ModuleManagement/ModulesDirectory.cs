using System.IO;
using System.Threading.Tasks;
using NuGet.Configuration;
using NuGet.Packaging;
using NuGet.Packaging.Core;

namespace Orcus.ModuleManagement
{
    public class ModulesDirectory : IModulesDirectory
    {
        public ModulesDirectory(VersionFolderPathResolver versionFolderPathResolver)
        {
            PackageSource = new PackageSource(versionFolderPathResolver.RootPath);
            VersionFolderPathResolver = versionFolderPathResolver;
        }

        public PackageSource PackageSource { get; }
        public VersionFolderPathResolver VersionFolderPathResolver { get; }

        public bool ModuleExists(PackageIdentity packageIdentity)
        {
            return File.Exists(
                VersionFolderPathResolver.GetPackageFilePath(packageIdentity.Id, packageIdentity.Version));
        }

        public Task DeleteModule(PackageIdentity packageIdentity)
        {
            var moduleDirectory = VersionFolderPathResolver.GetInstallPath(packageIdentity.Id, packageIdentity.Version);
            if (Directory.Exists(moduleDirectory))
            {
                Directory.Delete(moduleDirectory, true);
            }
            return Task.CompletedTask; //TODO
        }
    }
}