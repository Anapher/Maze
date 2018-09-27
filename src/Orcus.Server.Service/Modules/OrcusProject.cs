using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using Orcus.ModuleManagement;
using Orcus.ModuleManagement.Loader;
using Orcus.Server.Connection.Modules;
using Orcus.Server.Connection.Utilities;
using Orcus.Server.Service.Modules.Config;
using Architecture = Orcus.ModuleManagement.Loader.Architecture;

namespace Orcus.Server.Service.Modules
{
    public class OrcusProject : IModuleProject
    {
        private readonly IModulesConfig _modulesConfig;

        public OrcusProject(IModuleProjectConfig config, IModulesConfig modulesConfig, IModulesLock modulesLock)
        {
            var providers = Repository.Provider.GetCoreV3();
            PrimarySources = config.PrimarySources
                .Select(x => new SourceRepository(new PackageSource(x.AbsoluteUri), providers)).ToImmutableList();
            DependencySources = config.DependencySources
                .Select(x => new SourceRepository(new PackageSource(x.AbsoluteUri), providers)).ToImmutableList();

            ModulesDirectory = new ModulesDirectory(new VersionFolderPathResolver(config.Directory));
            LocalSourceRepository = new SourceRepository(ModulesDirectory.PackageSource, providers);

            _modulesConfig = modulesConfig;
            ModulesLock = modulesLock;
            FrameworkLibraries = config.Frameworks.ToDictionary(x => NuGetFramework.Parse(x.Key),
                x => PackageIdentityConvert.ToPackageIdentity(x.Value));

            Architecture = Environment.Is64BitProcess ? Architecture.x64 : Architecture.x86;
            Runtime = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? Runtime.Windows : Runtime.Linux;
            PrimaryPackages = modulesConfig.Modules; //very important because the file may change while the packages must consist
        }

        public NuGetFramework Framework { get; } = FrameworkConstants.CommonFrameworks.OrcusServer10;
        public Runtime Runtime { get; }
        public Architecture Architecture { get; }
        public IImmutableList<PackageIdentity> PrimaryPackages { get; }
        public IModulesLock ModulesLock { get; }

        public IImmutableDictionary<PackageIdentity, IImmutableList<PackageIdentity>> InstalledPackages =>
            ModulesLock.Modules.TryGetValue(Framework, out var packagesLock)
                ? (IImmutableDictionary<PackageIdentity, IImmutableList<PackageIdentity>>) packagesLock
                : ImmutableDictionary<PackageIdentity, IImmutableList<PackageIdentity>>.Empty;

        public IImmutableList<SourceRepository> PrimarySources { get; }
        public IImmutableList<SourceRepository> DependencySources { get; }
        public SourceRepository LocalSourceRepository { get; }
        public IModulesDirectory ModulesDirectory { get; }
        public IReadOnlyDictionary<NuGetFramework, PackageIdentity> FrameworkLibraries { get; }

        public Task<bool> InstallPackageAsync(PackageIdentity packageIdentity,
            DownloadResourceResult downloadResourceResult,
            CancellationToken token)
        {
            return Task.FromResult(true);
        }

        public Task<bool> UninstallPackageAsync(PackageIdentity packageIdentity, CancellationToken token)
        {
            return Task.FromResult(true);
        }

        public async Task SetServerModulesLock(IReadOnlyList<PackageIdentity> primaryModules, PackagesLock serverLock)
        {
            await _modulesConfig.Replace(primaryModules);
            await ModulesLock.Replace(new Dictionary<NuGetFramework, PackagesLock>{{Framework, serverLock}});
        }

        public Task AddModulesLock(NuGetFramework framework, PackagesLock packagesLock)
        {
            return ModulesLock.Add(framework, packagesLock);
        }
    }
}