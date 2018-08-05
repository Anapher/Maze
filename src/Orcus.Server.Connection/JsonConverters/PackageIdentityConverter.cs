using System;
using Newtonsoft.Json;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using Orcus.Server.Connection.Modules;

namespace Orcus.Server.Connection.JsonConverters
{
    public class PackageIdentityConvertera : JsonConverter<PackageIdentity>
    {
        public override void WriteJson(JsonWriter writer, PackageIdentity value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, PackageIdentityConverter.ToString(value));
        }

        public override PackageIdentity ReadJson(JsonReader reader, Type objectType, PackageIdentity existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var token = serializer.Deserialize<string>(reader);
            return PackageIdentityConverter.ToPackageIdentity(token);
        }
    }

    public class PackageIdentityConverter
    {
        public static string ToString(PackageIdentity packageIdentity)
        {
            var result = $"{packageIdentity.Id}";
            if (packageIdentity.HasVersion)
                result += "/" + packageIdentity.Version;

            return result;
        }

        public static PackageIdentity ToPackageIdentity(string s)
        {
            if (!s.Contains("/"))
                return new PackageIdentity(s, null);

            var split = s.Split(new[] {'/'}, 2);
            return new PackageIdentity(split[0], NuGetVersion.Parse(split[1]));
        }
    }
}