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
            PrimaryModules?.Select(PackageIdentityConvert.FromString).ToArray();

        public PackagesLock GetPackagesLock()
        {
            return new PackagesLock
            {
                Packages = PackagesLock?.ToImmutableDictionary(x => PackageIdentityConvert.FromString(x.Key),
                    x => (IImmutableList<PackageIdentity>) x.Value.Select(PackageIdentityConvert.FromString)
                        .ToImmutableList())
            };
        }
    }
}