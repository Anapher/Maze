using Newtonsoft.Json;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Versioning;
using Orcus.Server.Connection.JsonConverters;
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
            var test = "\"Orcus.Nora/1.2.4\"";
            var result = JsonConvert.DeserializeObject<PackageIdentity>(test, _jsonSettings);
            Assert.NotNull(result);
            Assert.Equal("1.2.4", result.Version.ToString());
            Assert.Equal("Orcus.Nora", result.Id);
        }

        [Fact]
        public void TestSerialize()
        {
            var packageIdentity = new PackageIdentity("Orcus.Nora", NuGetVersion.Parse("1.2.4"));
            var result = JsonConvert.SerializeObject(packageIdentity, _jsonSettings);
            Assert.NotNull(result);
            Assert.Equal("\"Orcus.Nora/1.2.4\"", result);
        }
    }
}