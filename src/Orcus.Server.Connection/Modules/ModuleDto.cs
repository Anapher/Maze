using System;
using System.Collections.Generic;
using NuGet.Packaging;
using NuGet.Packaging.Core;

namespace Orcus.Server.Connection.Modules
{
    public class ModuleDto
    {
        public PackageIdentity Id { get; set; }
        public string Title { get; set; }
        public string Authors { get; set; }
        public string Owners { get; set; }
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