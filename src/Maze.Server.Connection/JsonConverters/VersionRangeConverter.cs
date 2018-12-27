using System;
using Newtonsoft.Json;
using NuGet.Versioning;

namespace Orcus.Server.Connection.JsonConverters
{
    public class VersionRangeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            return VersionRange.Parse(serializer.Deserialize<string>(reader));
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(VersionRange);
        }
    }
}