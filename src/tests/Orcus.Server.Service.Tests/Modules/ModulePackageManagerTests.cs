using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.PackageManagement;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using Orcus.ModuleManagement;
using Orcus.Server.Connection.Modules;
using Orcus.Server.Service.Modules;
using Orcus.Server.Service.Tests.Utils;
using Xunit;

namespace Orcus.Server.Service.Tests.Modules
{
    public class ModulePackageManagerTests
    {
        [Fact]
        public async Task TestInstallModuleNoDependencies()
        {
            var sourceRepositoryProvider = TestSourceRepositoryUtility.CreateSourceRepositoryProvider(new[]
            {
                TestSourceRepositoryUtility.V3PackageSource,
                new PackageSource(new Uri(Path.GetFullPath("modules"), UriKind.Absolute).AbsoluteUri)
            });

            var token = CancellationToken.None;
            var logger = NullLogger.Instance;

            var sources = sourceRepositoryProvider.GetRepositories().ToList();

            var package = new SourcedPackageIdentity("Microsoft.AspNet.Razor", NuGetVersion.Parse("3.0.0"), sources.First().PackageSource.SourceUri);
            var testProject = new TestModuleProject(NuGetFramework.Parse("net10"), sources);
            var packageManager = new ModulePackageManager(testProject);

            var actions =
                await packageManager.PreviewInstallPackageAsync(package, new ResolutionContext(), logger, token);

            var resolvedAction = Assert.Single(actions);
            Assert.Equal(ResolvedActionType.Install, resolvedAction.Action);
            Assert.Equal(package.Id, resolvedAction.PackageIdentity.Id, StringComparer.OrdinalIgnoreCase);
            Assert.Equal(package.Version, resolvedAction.PackageIdentity.Version, VersionComparer.Default);
        }

        [Fact]
        public async Task TestInstallModuleDeepDependencies()
        {
            var sourceRepositoryProvider = TestSourceRepositoryUtility.CreateSourceRepositoryProvider(new[]
            {
                TestSourceRepositoryUtility.V3PackageSource,
                new PackageSource(new Uri(Path.GetFullPath("modules"), UriKind.Absolute).AbsoluteUri)
            });

            var token = CancellationToken.None;
            var logger = NullLogger.Instance;

            var sources = sourceRepositoryProvider.GetRepositories().ToList();

            var package = new SourcedPackageIdentity("NuGet.Client", NuGetVersion.Parse("4.2.0"), sources.First().PackageSource.SourceUri);
            var testProject =
                new TestModuleProject(NuGetFramework.Parse("net45"), sources);
            var packageManager = new ModulePackageManager(testProject);

            var actions =
                await packageManager.PreviewInstallPackageAsync(package, new ResolutionContext(), logger, token);

            //first layer dependency
            Assert.Contains(actions,
                x => x.PackageIdentity.Id.Equals("NuGet.Packaging", StringComparison.OrdinalIgnoreCase));

            //second layer dependency
            Assert.Contains(actions,
                x => x.PackageIdentity.Id.Equals("Newtonsoft.Json", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task TestInstallModuleDeepDependenciesKeepInstalledPrimaryPackageVersion()
        {
            var sourceRepositoryProvider = TestSourceRepositoryUtility.CreateSourceRepositoryProvider(new[]
            {
                TestSourceRepositoryUtility.V3PackageSource,
                new PackageSource(new Uri(Path.GetFullPath("modules"), UriKind.Absolute).AbsoluteUri)
            });

            var token = CancellationToken.None;
            var logger = NullLogger.Instance;

            var sources = sourceRepositoryProvider.GetRepositories().ToList();
            var nugetSourceUri = sources.First().PackageSource.SourceUri;

            var package = new SourcedPackageIdentity("NuGet.Client", NuGetVersion.Parse("4.2.0"), nugetSourceUri);
            var testProject =
                new TestModuleProject(NuGetFramework.Parse("net45"), sources)
                {
                    PrimaryPackages = new[]
                        {
                            new SourcedPackageIdentity("Newtonsoft.Json", NuGetVersion.Parse("11.0.1"), nugetSourceUri)
                        }
                        .ToImmutableArray()
                };
            var packageManager = new ModulePackageManager(testProject);

            var actions =
                await packageManager.PreviewInstallPackageAsync(package, new ResolutionContext(), logger, token);

            //first layer dependency
            Assert.Contains(actions,
                x => x.PackageIdentity.Id.Equals("NuGet.Packaging", StringComparison.OrdinalIgnoreCase));

            var jsonPackage = actions.FirstOrDefault(x =>
                x.PackageIdentity.Id.Equals("Newtonsoft.Json", StringComparison.OrdinalIgnoreCase));
            Assert.NotNull(jsonPackage);

            Assert.Equal("11.0.1", jsonPackage.PackageIdentity.Version.ToString());
        }

        [Fact]
        public async Task TestInstallModuleDeepDependenciesDontInstallExistingPackages()
        {
            var sourceRepositoryProvider = TestSourceRepositoryUtility.CreateSourceRepositoryProvider(new[]
            {
                TestSourceRepositoryUtility.V3PackageSource,
                new PackageSource(new Uri(Path.GetFullPath("modules"), UriKind.Absolute).AbsoluteUri)
            });

            var token = CancellationToken.None;
            var logger = NullLogger.Instance;

            var sources = sourceRepositoryProvider.GetRepositories().ToList();
            var nugetSourceUri = sources.First().PackageSource.SourceUri;

            var package = new SourcedPackageIdentity("NuGet.Client", NuGetVersion.Parse("4.2.0"), nugetSourceUri);
            var jsonPackage =
                new SourcedPackageIdentity("Newtonsoft.Json", NuGetVersion.Parse("11.0.1"), nugetSourceUri);

            var testProject =
                new TestModuleProject(NuGetFramework.Parse("net45"), sources)
                {
                    PrimaryPackages = new[] {jsonPackage}.ToImmutableArray(),
                    InstalledPackages =
                        new Dictionary<PackageIdentity, IReadOnlyList<PackageIdentity>>
                        {
                            {jsonPackage, new List<PackageIdentity>()}
                        }.ToImmutableDictionary()
                };
            var packageManager = new ModulePackageManager(testProject);

            var actions =
                await packageManager.PreviewInstallPackageAsync(package, new ResolutionContext(), logger, token);

            //first layer dependency
            Assert.Contains(actions,
                x => x.PackageIdentity.Id.Equals("NuGet.Packaging", StringComparison.OrdinalIgnoreCase));

            var jsonAction = actions.FirstOrDefault(x =>
                x.PackageIdentity.Id.Equals("Newtonsoft.Json", StringComparison.OrdinalIgnoreCase));
            Assert.Null(jsonAction);
        }

        [Fact]
        public async Task TestInstallModuleDeepDependenciesUpdatePrimaryPackage()
        {
            var sourceRepositoryProvider = TestSourceRepositoryUtility.CreateSourceRepositoryProvider(new[]
            {
                TestSourceRepositoryUtility.V3PackageSource,
                new PackageSource(new Uri(Path.GetFullPath("modules"), UriKind.Absolute).AbsoluteUri)
            });

            var token = CancellationToken.None;
            var logger = NullLogger.Instance;

            var sources = sourceRepositoryProvider.GetRepositories().ToList();
            var nugetSourceUri = sources.First().PackageSource.SourceUri;

            var package = new SourcedPackageIdentity("NuGet.Client", NuGetVersion.Parse("4.2.0"), nugetSourceUri);
            var jsonPackage =
                new SourcedPackageIdentity("Newtonsoft.Json", NuGetVersion.Parse("3.5.8"), nugetSourceUri);

            var testProject =
                new TestModuleProject(NuGetFramework.Parse("net45"), sources)
                {
                    PrimaryPackages = new[] { jsonPackage }.ToImmutableArray(),
                    InstalledPackages =
                        new Dictionary<PackageIdentity, IReadOnlyList<PackageIdentity>>
                        {
                            {jsonPackage, new List<PackageIdentity>()}
                        }.ToImmutableDictionary()
                };
            var packageManager = new ModulePackageManager(testProject);

            var actions =
                await packageManager.PreviewInstallPackageAsync(package, new ResolutionContext(), logger, token);

            //first layer dependency
            Assert.Contains(actions,
                x => x.PackageIdentity.Id.Equals("NuGet.Packaging", StringComparison.OrdinalIgnoreCase));

            var jsonActions = actions.Where(x =>
                x.PackageIdentity.Id.Equals("Newtonsoft.Json", StringComparison.OrdinalIgnoreCase)).ToList();
            Assert.Equal(2, jsonActions.Count);

            var uninstallAction = jsonActions.Single(x => x.Action == ResolvedActionType.Uninstall);
            Assert.Equal("3.5.8", uninstallAction.PackageIdentity.Version.ToString());

            var installAction = jsonActions.Single(x => x.Action != ResolvedActionType.Uninstall);
            Assert.NotEqual("3.5.8", installAction.PackageIdentity.Version.ToString());
        }

        [Fact]
        public async Task TestInstallModuleDeepDependenciesAndExecuteActions()
        {
            using (var testDir = TestDirectory.Create())
            {
                var modulesDirectory = new ModulesDirectory(new VersionFolderPathResolverFlat(testDir.Path));

                var sourceRepositoryProvider = TestSourceRepositoryUtility.CreateSourceRepositoryProvider(new[]
                {
                    TestSourceRepositoryUtility.V3PackageSource,
                    modulesDirectory.PackageSource
                });

                var token = CancellationToken.None;
                var logger = NullLogger.Instance;

                var sources = sourceRepositoryProvider.GetRepositories().ToList();
                var nugetSourceUri = sources.First().PackageSource.SourceUri;

                var package = new SourcedPackageIdentity("NuGet.Client", NuGetVersion.Parse("4.2.0"), nugetSourceUri);
                var jsonPackage =
                    new SourcedPackageIdentity("Newtonsoft.Json", NuGetVersion.Parse("11.0.1"), nugetSourceUri);

                var testProject =
                    new TestModuleProject(NuGetFramework.Parse("net45"), sources)
                    {
                        PrimaryPackages = new[] { jsonPackage }.ToImmutableArray(),
                        InstalledPackages =
                            new Dictionary<PackageIdentity, IReadOnlyList<PackageIdentity>>
                            {
                                {jsonPackage, new List<PackageIdentity>()}
                            }.ToImmutableDictionary(),
                        ModulesDirectory = modulesDirectory
                    };
                var packageManager = new ModulePackageManager(testProject);

                var actions =
                    await packageManager.PreviewInstallPackageAsync(package, new ResolutionContext(), logger, token);

                await packageManager.ExecuteActionsAsync(actions, new PackageDownloadContext(new SourceCacheContext(), testDir.Path, true),
                    token);

                var directory = testDir.Info;
                Assert.Empty(directory.EnumerateFiles());
                Assert.Equal(actions.Count(), directory.EnumerateDirectories().Count());
            }
        }
    }

    public class TestModuleProject : IModuleProject
    {
        public TestModuleProject(NuGetFramework framework, IReadOnlyCollection<SourceRepository> sources)
        {
            Framework = framework;
            Sources = sources.ToImmutableList();

            LocalSourceRepository = sources.First(x => x.PackageSource.IsLocal);
        }

        public NuGetFramework Framework { get; }

        public IImmutableList<SourcedPackageIdentity> PrimaryPackages { get; set; } =
            ImmutableList<SourcedPackageIdentity>.Empty;

        public IImmutableDictionary<PackageIdentity, IReadOnlyList<PackageIdentity>> InstalledPackages { get; set; } =
            ImmutableDictionary<PackageIdentity, IReadOnlyList<PackageIdentity>>.Empty;

        public IImmutableList<SourceRepository> Sources { get; }
        public SourceRepository LocalSourceRepository { get; }
        public IModulesDirectory ModulesDirectory { get; set; }
        public AsyncLock BatchLock { get; } = new AsyncLock();

        public Task<bool> InstallPackageAsync(PackageIdentity packageIdentity, DownloadResourceResult downloadResourceResult,
            CancellationToken token)
        {
            return Task.FromResult(true);
        }

        public Task<bool> UninstallPackageAsync(PackageIdentity packageIdentity, CancellationToken token)
        {
            return Task.FromResult(true);
        }

        public Task SetModuleLock(IReadOnlyList<SourcedPackageIdentity> primaryPackages, IReadOnlyDictionary<PackageIdentity, IReadOnlyList<PackageIdentity>> serverLock, IReadOnlyDictionary<PackageIdentity, IReadOnlyList<PackageIdentity>> adminLock,
            IReadOnlyDictionary<PackageIdentity, IReadOnlyList<PackageIdentity>> clientLock)
        {
            throw new NotImplementedException();
        }
    }
}