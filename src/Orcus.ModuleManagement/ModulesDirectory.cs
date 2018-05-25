using System.IO;
using System.Threading.Tasks;
using NuGet.Configuration;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Versioning;

namespace Orcus.ModuleManagement
{
    public interface IModulesDirectory
    {
        PackageSource PackageSource { get; }
        VersionFolderPathResolver VersionFolderPathResolver { get; }

        bool ModuleExists(PackageIdentity packageIdentity);
        Task DeleteModule(PackageIdentity packageIdentity);
    }

    public class VersionFolderPathResolverFlat : VersionFolderPathResolver
    {
        public VersionFolderPathResolverFlat(string rootPath) : base(rootPath, false)
        {
        }

        public override string GetPackageDirectory(string packageId, NuGetVersion version)
        {
            return GetVersionListDirectory(packageId) + "." + version.ToNormalizedString();
        }
    }

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