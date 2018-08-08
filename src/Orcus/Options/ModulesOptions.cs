using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NuGet.Packaging.Core;
using Orcus.Server.Connection.Modules;
using Orcus.Server.Connection.Utilities;

namespace Orcus.Options
{
    public class ModulesOptions
    {
        public string LocalPath { get; set; }
        public string[] PrimaryModules { get; set; }
        public Dictionary<string, string[]> PackagesLock { get; set; }

        public PackageIdentity[] GetPrimaryModules() =>
            PrimaryModules?.Select(PackageIdentityConvert.ToPackageIdentity).ToArray();

        public PackagesLock GetPackagesLock()
        {
            return new PackagesLock(PackagesLock?.ToImmutableDictionary(x => PackageIdentityConvert.ToPackageIdentity(x.Key),
                x => (IImmutableList<PackageIdentity>) x.Value.Select(PackageIdentityConvert.ToPackageIdentity)
                    .ToImmutableList()));
        }
    }
}