using Newtonsoft.Json;
using NuGet.Protocol;
using Maze.Server.Connection.JsonConverters;

namespace Maze.Server.Extensions
{
    public static class JsonOptionsExtensions
    {
        public static void Configure(this JsonSerializerSettings settings)
        {
            settings.Converters.Add(new NuGetVersionConverter());
            settings.Converters.Add(new SemanticVersionConverter());
            settings.Converters.Add(new VersionRangeConverter());
            settings.Converters.Add(new NuGetFrameworkConverter());
            settings.Converters.Add(new PackageIdentityConverter());
        }
    }
}