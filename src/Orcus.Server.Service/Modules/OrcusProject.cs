using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using Orcus.ModuleManagement;
using Orcus.Server.Connection.Modules;
using Orcus.Server.Service.Modules.Config;

namespace Orcus.Server.Service.Modules
{
    public class OrcusProject : IModuleProject
    {
        private readonly IModulesConfig _modulesConfig;
        private readonly IModulesLock _modulesLock;

        public OrcusProject(IEnumerable<Uri> primarySources, IEnumerable<Uri> dependencySources,
            string modulesDirectory, IModulesConfig modulesConfig, IModulesLock modulesLock)
        {
            var providers = Repository.Provider.GetCoreV3();
            PrimarySources = primarySources
                .Select(x => new SourceRepository(new PackageSource(x.AbsoluteUri), providers)).ToImmutableList();
            DependencySources = dependencySources
                .Select(x => new SourceRepository(new PackageSource(x.AbsoluteUri), providers)).ToImmutableList();

            ModulesDirectory = new ModulesDirectory(new VersionFolderPathResolver(modulesDirectory));
            LocalSourceRepository = new SourceRepository(ModulesDirectory.PackageSource, providers);

            _modulesConfig = modulesConfig;
            _modulesLock = modulesLock;
        }

        public NuGetFramework Framework { get; } = FrameworkConstants.CommonFrameworks.OrcusServer10;
        public IImmutableList<PackageIdentity> PrimaryPackages => _modulesConfig.Modules;

        public IImmutableDictionary<PackageIdentity, IImmutableList<PackageIdentity>> InstalledPackages =>
            _modulesLock.Modules[Framework].Packages;

        public IImmutableList<SourceRepository> PrimarySources { get; }
        public IImmutableList<SourceRepository> DependencySources { get; }
        public SourceRepository LocalSourceRepository { get; }
        public IModulesDirectory ModulesDirectory { get; }

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
            await _modulesLock.Replace(new Dictionary<NuGetFramework, PackagesLock>{{Framework, serverLock}});
        }

        public Task AddModulesLock(NuGetFramework framework, PackagesLock packagesLock)
        {
            return _modulesLock.Add(framework, packagesLock);
        }
    }
}