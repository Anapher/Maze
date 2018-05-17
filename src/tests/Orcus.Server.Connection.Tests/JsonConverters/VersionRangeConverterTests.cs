using Newtonsoft.Json;
using NuGet.Versioning;
using Orcus.Server.Connection.JsonConverters;
using Xunit;

namespace Orcus.Server.Connection.Tests.JsonConverters
{
    public class VersionRangeConverterTests
    {
        private readonly JsonSerializerSettings _jsonSettings;

        public VersionRangeConverterTests()
        {
            _jsonSettings = new JsonSerializerSettings();
            _jsonSettings.Converters.Add(new VersionRangeConverter());
        }

        [Fact]
        public void TestDeserialize()
        {
            var test = "\"(, 1.0.0)\"";
            var result = JsonConvert.DeserializeObject<VersionRange>(test, _jsonSettings);
            Assert.NotNull(result);
            Assert.Equal("(, 1.0.0)", result.ToString());
        }

        [Fact]
        public void TestSerialize()
        {
            var versionRange = VersionRange.Parse("(,1.0)");
            var result = JsonConvert.SerializeObject(versionRange, _jsonSettings);
            Assert.NotNull(result);
            Assert.Equal("\"(, 1.0.0)\"", result);
        }
    }
}