using System;
using NuGet.Packaging.Core;

namespace Orcus.Server.Service.Modules.Extensions
{
    public static class PackageIdentityExtensions
    {
        public static bool IsSameId(this PackageIdentity packageIdentity, PackageIdentity other)
        {
            return string.Equals(packageIdentity.Id, other.Id, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsSameId(this PackageIdentity packageIdentity, PackageDependency other)
        {
            return string.Equals(packageIdentity.Id, other.Id, StringComparison.OrdinalIgnoreCase);
        }
    }
}