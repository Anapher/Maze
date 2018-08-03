using NuGet.Packaging.Core;
using NuGet.Versioning;

namespace Orcus.Server.Connection.Utilities
{
    public static class PackageIdentityConvert
    {
        public static string ConvertToString(this PackageIdentity packageId) => $"{packageId.Id}/{packageId.Version}";

        public static PackageIdentity FromString(string s)
        {
            var split = s.Split(new[] {'/'}, 2);
            return new PackageIdentity(split[0], NuGetVersion.Parse(split[1]));
        }
    }
}