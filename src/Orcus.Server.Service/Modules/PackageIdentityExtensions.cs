using NuGet.Packaging.Core;

namespace Orcus.Server.Service.Modules
{
    public static class PackageIdentityExtensions
    {
        public static string GetFilename(this PackageIdentity packageIdentity)
        {
            return packageIdentity.Id + "." + packageIdentity.Version.Version;
        }
    }
}