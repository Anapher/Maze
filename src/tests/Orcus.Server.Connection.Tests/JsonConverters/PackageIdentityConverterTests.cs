using System;
using Newtonsoft.Json;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Versioning;
using Orcus.Server.Connection.JsonConverters;
using Orcus.Server.Connection.Modules;
using Xunit;

namespace Orcus.Server.Connection.Tests.JsonConverters
{
    public class PackageIdentityConverterTests
    {
        public PackageIdentityConverterTests()
        {
            _jsonSettings = new JsonSerializerSettings();
            _jsonSettings.Converters.Add(new PackageIdentityConverter());
            _jsonSettings.Converters.Add(new NuGetVersionConverter());
        }

        private readonly JsonSerializerSettings _jsonSettings;

        [Fact]
        public void TestDeserialize()
        {
            var test = "{\"Id\":\"Orcus.Nora\",\"Version\":\"1.2.4\"}";
            var result = JsonConvert.DeserializeObject<PackageIdentity>(test, _jsonSettings);
            Assert.NotNull(result);
            Assert.Equal("1.2.4", result.Version.ToString());
            Assert.Equal("Orcus.Nora", result.Id);
        }

        [Fact]
        public void TestDeserializeSourcedExplicitSource()
        {
            var test =
                "{\"Id\":\"Orcus.Nora\",\"Version\":\"1.2.4\",\"SourceRepository\":\"https://api.nora.org/v3/index.json\"}";
            var result = JsonConvert.DeserializeObject<SourcedPackageIdentity>(test, _jsonSettings);
            Assert.NotNull(result);
            Assert.Equal("1.2.4", result.Version.ToString());
            Assert.Equal("Orcus.Nora", result.Id);
            Assert.Equal(new Uri("https://api.nora.org/v3/index.json"), result.SourceRepository);
        }

        [Fact]
        public void TestDeserializeSourcedNoExplicitSource()
        {
            var test = "{\"Id\":\"Orcus.Nora\",\"Version\":\"1.2.4\"}";
            var result = JsonConvert.DeserializeObject<SourcedPackageIdentity>(test, _jsonSettings);
            Assert.NotNull(result);
            Assert.Equal("1.2.4", result.Version.ToString());
            Assert.Equal("Orcus.Nora", result.Id);
            Assert.Equal(OfficalOrcusRepository.Uri, result.SourceRepository);
        }

        [Fact]
        public void TestSerialize()
        {
            var packageIdentity = new PackageIdentity("Orcus.Nora", NuGetVersion.Parse("1.2.4"));
            var result = JsonConvert.SerializeObject(packageIdentity, _jsonSettings);
            Assert.NotNull(result);
            Assert.Equal("{\"Id\":\"Orcus.Nora\",\"Version\":\"1.2.4\"}", result);
        }

        [Fact]
        public void TestSerializeSourcedDifferentSource()
        {
            var packageIdentity = new SourcedPackageIdentity("Orcus.Nora", NuGetVersion.Parse("1.2.4"),
                new Uri("https://api.nora.org/v3/index.json"));

            var result = JsonConvert.SerializeObject(packageIdentity, _jsonSettings);
            Assert.NotNull(result);
            Assert.Equal(
                "{\"Id\":\"Orcus.Nora\",\"Version\":\"1.2.4\",\"SourceRepository\":\"https://api.nora.org/v3/index.json\"}",
                result);
        }

        [Fact]
        public void TestSerializeSourcedOfficalSource()
        {
            var packageIdentity =
                new SourcedPackageIdentity("Orcus.Nora", NuGetVersion.Parse("1.2.4"), OfficalOrcusRepository.Uri);

            var result = JsonConvert.SerializeObject(packageIdentity, _jsonSettings);
            Assert.NotNull(result);
            Assert.Equal("{\"Id\":\"Orcus.Nora\",\"Version\":\"1.2.4\"}", result);
        }
    }
}