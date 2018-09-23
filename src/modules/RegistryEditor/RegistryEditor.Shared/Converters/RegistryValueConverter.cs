using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using RegistryEditor.Shared.Dtos;

namespace RegistryEditor.Shared.Converters
{
    internal class RegistryValueConverter : CustomCreationConverter<RegistryValue>
    {
        private RegistryValueType _type;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jobj = JToken.ReadFrom(reader);
            _type = jobj["type"].ToObject<RegistryValueType>();

            return base.ReadJson(jobj.CreateReader(), objectType, existingValue, serializer);
        }

        public override RegistryValue Create(Type objectType)
        {
            switch (_type)
            {
                case RegistryValueType.String:
                    return new StringRegistryValue();
                case RegistryValueType.Binary:
                    return new BinaryRegistryValue();
                case RegistryValueType.DWord:
                    return new DWordRegistryValue();
                case RegistryValueType.QWord:
                    return new QWordRegistryValue();
                case RegistryValueType.MultiString:
                    return new MultiStringRegistryValue();
                case RegistryValueType.ExpandableString:
                    return new ExpandableStringRegistryValue();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
