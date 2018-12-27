using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Newtonsoft.Json;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using Orcus.Server.Connection.Modules;
using Orcus.Server.Connection.Utilities;

namespace Orcus.Server.Connection.JsonConverters
{
    public class PackageLockConverter : JsonConverter<PackagesLock>
    {
        public override void WriteJson(JsonWriter writer, PackagesLock value, JsonSerializer serializer)
        {
            serializer.Serialize(writer,
                value.ToDictionary(x => PackageIdentityConvert.ToString(x.Key),
                    x => x.Value.ToDictionary(y => y.Id, y => y.Version.ToString())));
        }

        public override PackagesLock ReadJson(JsonReader reader, Type objectType, PackagesLock existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            var result = serializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(reader);
            return new PackagesLock(result.ToDictionary(x => PackageIdentityConvert.ToPackageIdentity(x.Key),
                x => (IImmutableList<PackageIdentity>) x.Value
                    .Select(y => new PackageIdentity(y.Key, NuGetVersion.Parse(y.Value))).ToImmutableList()));
        }
    }
}