using NuGet.Packaging.Core;
using NuGet.Versioning;

namespace Orcus.Server.Connection.Utilities
{
    public static class PackageIdentityConvert
    {
        public static string ToString(PackageIdentity packageIdentity)
        {
            var result = $"{packageIdentity.Id}";
            if (packageIdentity.HasVersion)
                result += "/" + packageIdentity.Version;

            return result;
        }

        public static PackageIdentity ToPackageIdentity(string s)
        {
            if (!s.Contains("/"))
                return new PackageIdentity(s, null);

            var split = s.Split(new[] { '/' }, 2);
            return new PackageIdentity(split[0], NuGetVersion.Parse(split[1]));
        }
    }
}