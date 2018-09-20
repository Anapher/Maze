using System;
using System.Collections.Generic;
using System.Text;
using SystemInformation.Shared.Dtos;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace SystemInformation.Shared.Converters
{
    internal class SystemInfoDtoConverter : CustomCreationConverter<SystemInfoDto>
    {
        private ValueDtoType _type;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var jobj = JToken.ReadFrom(reader);
            _type = jobj["type"].ToObject<ValueDtoType>();

            return base.ReadJson(jobj.CreateReader(), objectType, existingValue, serializer);
        }

        public override SystemInfoDto Create(Type objectType)
        {
            switch (_type)
            {
                    
            }
        }
    }
}
