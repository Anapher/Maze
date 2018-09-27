using Newtonsoft.Json;
using NuGet.Protocol;
using Orcus.Server.Connection.JsonConverters;

namespace Orcus.Server.Extensions
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