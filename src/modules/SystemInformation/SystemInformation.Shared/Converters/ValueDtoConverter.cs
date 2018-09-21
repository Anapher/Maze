using System;
using SystemInformation.Shared.Dtos.Value;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace SystemInformation.Shared.Converters
{
    internal class ValueDtoConverter : CustomCreationConverter<ValueDto>
    {
        private ValueDtoType _type;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jobj = JToken.ReadFrom(reader);
            _type = jobj["type"].ToObject<ValueDtoType>();

            return base.ReadJson(jobj.CreateReader(), objectType, existingValue, serializer);
        }

        public override ValueDto Create(Type objectType)
        {
            switch (_type)
            {
                case ValueDtoType.Text:
                    return new TextValueDto();
                case ValueDtoType.TranslatedText:
                    return new TranslatedTextValueDto();
                case ValueDtoType.Number:
                    return new NumberValueDto();
                case ValueDtoType.DataSize:
                    return new DataSizeValueDto();
                case ValueDtoType.Progress:
                    return new ProgressValueDto();
                case ValueDtoType.Culture:
                    return new CultureValueDto();
                case ValueDtoType.DateTime:
                    return new DateTimeValueDto();
                case ValueDtoType.Boolean:
                    return new BoolValueDto();
                case ValueDtoType.Header:
                    return HeaderValueDto.Instance;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}