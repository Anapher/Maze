using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using RegistryEditor.Shared.Dtos;

namespace RegistryEditor.Shared.Converters
{
    internal class RegistryValueConverter : CustomCreationConverter<RegistryValueDto>
    {
        private RegistryValueType _type;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jobj = JToken.ReadFrom(reader);
            _type = jobj["type"].ToObject<RegistryValueType>();

            return base.ReadJson(jobj.CreateReader(), objectType, existingValue, serializer);
        }

        public override RegistryValueDto Create(Type objectType)
        {
            switch (_type)
            {
                case RegistryValueType.String:
                    return new StringRegistryValueDto();
                case RegistryValueType.Binary:
                    return new BinaryRegistryValueDto();
                case RegistryValueType.DWord:
                    return new DWordRegistryValueDto();
                case RegistryValueType.QWord:
                    return new QWordRegistryValueDto();
                case RegistryValueType.MultiString:
                    return new MultiStringRegistryValueDto();
                case RegistryValueType.ExpandableString:
                    return new ExpandableStringRegistryValueDto();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}