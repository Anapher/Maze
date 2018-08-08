using System;
using Newtonsoft.Json;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using Orcus.Server.Connection.Modules;
using Orcus.Server.Connection.Utilities;

namespace Orcus.Server.Connection.JsonConverters
{
    public class PackageIdentityConverter : JsonConverter<PackageIdentity>
    {
        public override void WriteJson(JsonWriter writer, PackageIdentity value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, PackageIdentityConvert.ToString(value));
        }

        public override PackageIdentity ReadJson(JsonReader reader, Type objectType, PackageIdentity existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var token = serializer.Deserialize<string>(reader);
            return PackageIdentityConvert.ToPackageIdentity(token);
        }
    }
}