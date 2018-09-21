using SystemInformation.Shared.Converters;
using Newtonsoft.Json;

namespace SystemInformation.Shared.Dtos.Value
{
    [JsonConverter(typeof(ValueDtoConverter))]
    public abstract class ValueDto
    {
        public abstract ValueDtoType Type { get; }
    }
}