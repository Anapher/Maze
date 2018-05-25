using System;
using System.Collections.Generic;
using System.Linq;
using NuGet.Packaging.Core;
using NuGet.Versioning;

namespace Orcus.Administration.Modules.Extensions
{
    /// <summary>
    ///     Common package queries implementes as extension methods
    /// </summary>
    public static class PackageCollectionExtensions
    {
        public static NuGetVersion[] GetPackageVersions(this IEnumerable<PackageIdentity> packages, string packageId)
        {
            return packages
                .Where(p => StringComparer.OrdinalIgnoreCase.Equals(p.Id, packageId))
                .Select(p => p.Version)
                .ToArray();
        }

        public static IEnumerable<IGrouping<string, NuGetVersion>> GroupById(this IEnumerable<PackageIdentity> packages)
        {
            return packages
                .GroupBy(p => p.Id, p => p.Version, StringComparer.OrdinalIgnoreCase);
        }

        public static PackageIdentity[] GetLatest(this IEnumerable<PackageIdentity> packages)
        {
            return packages
                .GroupById()
                .Select(g => new PackageIdentity(g.Key, g.MaxOrDefault()))
                .ToArray();
        }

        public static PackageIdentity[] GetEarliest(this IEnumerable<PackageIdentity> packages)
        {
            return packages
                .GroupById()
                .Select(g => new PackageIdentity(g.Key, g.MinOrDefault()))
                .ToArray();
        }
    }
}