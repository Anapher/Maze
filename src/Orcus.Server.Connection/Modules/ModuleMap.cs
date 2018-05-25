using System.Collections.Generic;
using NuGet.Packaging.Core;

namespace Orcus.Server.Connection.Modules
{
    public class ModuleMap
    {
        public IReadOnlyList<SourcedPackageIdentity> PrimaryModules { get; set; }
        public IDictionary<PackageIdentity, IReadOnlyList<PackageIdentity>> PackageDependencies { get; set; }
    }
}