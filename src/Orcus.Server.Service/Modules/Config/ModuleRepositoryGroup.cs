using System;
using System.Collections.Generic;
using NuGet.Packaging.Core;

namespace Orcus.Server.Service.Modules.Config
{
    public class ModuleRepositoryGroup
    {
        public Uri Source { get; set; }
        public List<PackageIdentity> Modules { get; set; }
    }
}