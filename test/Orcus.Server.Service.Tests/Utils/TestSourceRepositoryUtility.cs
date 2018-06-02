using System;
using System.Collections.Generic;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace Orcus.Server.Service.Tests.Utils
{
    public class TestSourceRepositoryUtility
    {
        public static PackageSource V3PackageSource = new PackageSource("https://api.nuget.org/v3/index.json", "v3");
        public IEnumerable<Lazy<INuGetResourceProvider>> ResourceProviders { get; private set; }

        private void Initialize()
        {
            ResourceProviders = Repository.Provider.GetCoreV3();
        }

        public static SourceRepositoryProvider CreateV3SourceRepositoryProvider()
        {
            return CreateSourceRepositoryProvider(new List<PackageSource> {V3PackageSource});
        }

        public static SourceRepositoryProvider CreateSourceRepositoryProvider(IEnumerable<PackageSource> packageSources)
        {
            var thisUtility = new TestSourceRepositoryUtility();
            thisUtility.Initialize();
            var packageSourceProvider = new TestPackageSourceProvider(packageSources);

            var sourceRepositoryProvider =
                new SourceRepositoryProvider(packageSourceProvider, thisUtility.ResourceProviders);
            return sourceRepositoryProvider;
        }
    }

    /// <summary>
    ///     Provider that only returns V3 as a source
    /// </summary>
    public class TestPackageSourceProvider : IPackageSourceProvider
    {
        public TestPackageSourceProvider(IEnumerable<PackageSource> packageSources)
        {
            PackageSources = packageSources;
        }

        private IEnumerable<PackageSource> PackageSources { get; set; }

        public void DisablePackageSource(PackageSource source)
        {
            source.IsEnabled = false;
        }

        public bool IsPackageSourceEnabled(PackageSource source)
        {
            return true;
        }

        public IEnumerable<PackageSource> LoadPackageSources()
        {
            return PackageSources;
        }

        public event EventHandler PackageSourcesChanged;

        public void SavePackageSources(IEnumerable<PackageSource> sources)
        {
            PackageSources = sources;
            PackageSourcesChanged?.Invoke(this, null);
        }

        public string ActivePackageSourceName => throw new NotImplementedException();

        public string DefaultPushSource => throw new NotImplementedException();

        public void SaveActivePackageSource(PackageSource source)
        {
            throw new NotImplementedException();
        }
    }
}