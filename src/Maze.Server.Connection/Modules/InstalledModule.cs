using System.Collections.Generic;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Versioning;

namespace Orcus.Server.Connection.Modules
{
    public class InstalledModule : PackageIdentity
    {
        public InstalledModule(string id, NuGetVersion version) : base(id, version)
        {
        }

        public string Title { get; set; }
        public string Authors { get; set; }
        public string LicenseUrl { get; set; }
        public string ProjectUrl { get; set; }
        public string IconUrl { get; set; }
        public string Description { get; set; }
        public string Summary { get; set; }
        public string Copyright { get; set; }

        public bool IsServerLoadable { get; set; }
        public bool IsAdministrationLoadable { get; set; }
        public bool IsClientLoadable { get; set; }

        public List<PackageDependencyGroup> Dependencies { get; set; }
    }
}