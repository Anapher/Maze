using System.Threading.Tasks;
using NuGet.Configuration;
using NuGet.Packaging;
using NuGet.Packaging.Core;

namespace Orcus.ModuleManagement
{
    public interface IModulesDirectory
    {
        PackageSource PackageSource { get; }
        VersionFolderPathResolver VersionFolderPathResolver { get; }

        bool ModuleExists(PackageIdentity packageIdentity);
        Task DeleteModule(PackageIdentity packageIdentity);
    }
}