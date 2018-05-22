using System;
using System.Collections.Generic;
using System.Linq;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace Orcus.Server.Service.Modules
{
    public static class PrunePackageTreeExtensions
    {
        /// <summary>
        ///     Remove all versions of a package id from the list, except for the target version
        /// </summary>
        public static IEnumerable<SourcePackageDependencyInfo> RemoveAllVersionsForIdExcept(
            IEnumerable<SourcePackageDependencyInfo> packages, IEnumerable<PackageIdentity> targets)
        {
            var targetsDict = targets.ToDictionary(x => x.Id, x => x.Version, StringComparer.OrdinalIgnoreCase);
            var comparer = VersionComparer.VersionRelease;

            return packages.Where(p =>
                !targetsDict.TryGetValue(p.Id, out var version) || comparer.Equals(p.Version, version));
        }

        public static IEnumerable<SourcePackageDependencyInfo> PruneDowngrades(IEnumerable<SourcePackageDependencyInfo> packages, IEnumerable<PackageIdentity> packageReferences)
        {
            // prune every package that is less that the currently installed package

            var installed = packageReferences.ToDictionary(x => x.Id, x => x.Version, StringComparer.OrdinalIgnoreCase);
            return packages.Where(package =>
                !package.HasVersion || !installed.TryGetValue(package.Id, out var packageVersion) || packageVersion <= package.Version);
        }
    }
}