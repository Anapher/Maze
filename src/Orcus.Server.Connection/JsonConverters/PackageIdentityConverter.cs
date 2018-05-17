using System;
using Newtonsoft.Json;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using Orcus.Server.Connection.Modules;

namespace Orcus.Server.Connection.JsonConverters
{
    public class PackageIdentityConverter : JsonConverter<PackageIdentity>
    {
        public override void WriteJson(JsonWriter writer, PackageIdentity value, JsonSerializer serializer)
        {
            //ignore property HasVersion
            var wrapper = new PackageIdentityWrapper {Id = value.Id, Version = value.Version};
            if (value is SourcedPackageIdentity sourcedPackageIdentity)
                wrapper.SourceRepository = sourcedPackageIdentity.SourceRepository;

            serializer.Serialize(writer, wrapper);
        }

        public override PackageIdentity ReadJson(JsonReader reader, Type objectType, PackageIdentity existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var result = serializer.Deserialize<PackageIdentityWrapper>(reader);
            if (objectType == typeof(SourcedPackageIdentity))
                return new SourcedPackageIdentity(result.Id, result.Version, result.SourceRepository);

            return new PackageIdentity(result.Id, result.Version);
        }

        private class PackageIdentityWrapper
        {
            public string Id { get; set; }
            public NuGetVersion Version { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public Uri SourceRepository { get; set; } = OfficalOrcusRepository.Uri;

            public bool ShouldSerializeSourceRepository() => SourceRepository != OfficalOrcusRepository.Uri;
        }
    }
}