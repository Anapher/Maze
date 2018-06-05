using System.Collections.Immutable;
using NuGet.Packaging.Core;

namespace Orcus.Server.Connection.Modules
{
    public class PackagesLock
    {
        public IImmutableDictionary<PackageIdentity, IImmutableList<PackageIdentity>> Packages { get; set; }
    }
}