using NuGet.Packaging;
using NuGet.Versioning;

namespace Maze.ModuleManagement
{
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
}