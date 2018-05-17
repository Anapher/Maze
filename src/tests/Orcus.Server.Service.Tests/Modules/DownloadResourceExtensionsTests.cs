using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using Orcus.Server.Service.Modules;
using Xunit;

namespace Orcus.Server.Service.Tests.Modules
{
    public class DownloadResourceExtensionsTests
    {
        [Fact]
        public void CheckMethodExists()
        {
            var type = typeof(DownloadResourceV3);
            var method = type.GetMethod("GetDownloadUrl", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(method);
            Assert.Equal(typeof(Task<Uri>), method.ReturnType);

            var parameters = method.GetParameters();
            Assert.Equal(3, parameters.Length);
            Assert.Equal(typeof(PackageIdentity), parameters[0].ParameterType);
            Assert.Equal(typeof(ILogger), parameters[1].ParameterType);
            Assert.Equal(typeof(CancellationToken), parameters[2].ParameterType);
        }

        [Fact]
        public async Task TestDownloadResourceIsV3()
        {
            PackageSource packageSource = new PackageSource("https://api.nuget.org/v3/index.json");
            List<Lazy<INuGetResourceProvider>> providers = new List<Lazy<INuGetResourceProvider>>();
            providers.AddRange(Repository.Provider.GetCoreV3());

            SourceRepository sourceRepository = new SourceRepository(packageSource, providers);

            var downloadResource = await sourceRepository.GetResourceAsync<DownloadResource>();
            Assert.IsType<DownloadResourceV3>(downloadResource);
        }

        [Fact]
        public async Task TestGetDownloadUrl()
        {
            PackageSource packageSource = new PackageSource("https://api.nuget.org/v3/index.json");
            List<Lazy<INuGetResourceProvider>> providers = new List<Lazy<INuGetResourceProvider>>();
            providers.AddRange(Repository.Provider.GetCoreV3());

            SourceRepository sourceRepository = new SourceRepository(packageSource, providers);

            var downloadResource = await sourceRepository.GetResourceAsync<DownloadResource>();
            var uri = await downloadResource.GetDownloadUrl(new PackageIdentity("Newtonsoft.Json", NuGetVersion.Parse("11.0.2")),
                new NullLogger(), CancellationToken.None);

            Assert.Equal(
                new Uri("https://api.nuget.org/v3-flatcontainer/newtonsoft.json/11.0.2/newtonsoft.json.11.0.2.nupkg"),
                uri);
        }
    }
}