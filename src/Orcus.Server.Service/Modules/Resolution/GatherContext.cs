using System.Collections.Generic;
using NuGet.Common;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using Orcus.Server.Service.Modules.PackageManagement;

namespace Orcus.Server.Service.Modules.Resolution
{
    public class GatherContext
    {
        /// <summary>
        ///     Resolution context containing the GatherCache and DependencyBehavior.
        /// </summary>
        public ResolutionContext ResolutionContext { get; set; }

        /// <summary>
        ///     Primary sources - Primary targets must exist here.
        /// </summary>
        public IReadOnlyList<SourceRepository> PrimarySources { get; set; }

        /// <summary>
        ///     Sources for dependencies (NuGet, ...)
        /// </summary>
        public IReadOnlyList<SourceRepository> DependencySources { get; set; }

        /// <summary>
        ///     Packages folder
        /// </summary>
        public SourceRepository PackagesFolderSource { get; set; }

        /// <summary>
        ///     Target modules. Version may be null
        /// </summary>
        public IReadOnlyList<PackageIdentity> PrimaryTargets { get; set; }

        public NuGetFramework PrimaryFramework { get; set; }
        public NuGetFramework DependencyFramework { get; set; }

        public ILogger Log { get; set; }
        public bool AllowDowngrades { get; set; }
    }
}