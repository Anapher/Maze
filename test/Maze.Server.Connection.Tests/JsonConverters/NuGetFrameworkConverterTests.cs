using Newtonsoft.Json;
using NuGet.Frameworks;
using Maze.Server.Connection.JsonConverters;
using Xunit;

namespace Maze.Server.Connection.Tests.JsonConverters
{
    public class NuGetFrameworkConverterTests
    {
        private readonly JsonSerializerSettings _jsonSettings;

        public NuGetFrameworkConverterTests()
        {
            _jsonSettings = new JsonSerializerSettings();
            _jsonSettings.Converters.Add(new NuGetFrameworkConverter());
        }

        [Fact]
        public void TestDeserialize()
        {
            var test = "\"Windows\"";
            var result = JsonConvert.DeserializeObject<NuGetFramework>(test, _jsonSettings);
            Assert.NotNull(result);
            Assert.Equal(NuGetFramework.Parse("win"), result);
        }

        [Fact]
        public void TestSerialize()
        {
            var framework = NuGetFramework.Parse("win");
            var result = JsonConvert.SerializeObject(framework, _jsonSettings);
            Assert.NotNull(result);
            Assert.Equal("\"Windows,Version=v0.0\"", result);
        }
    }
}