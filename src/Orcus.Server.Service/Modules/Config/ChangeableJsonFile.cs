using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Orcus.Server.Service.Modules.Config
{
    public abstract class ChangeableJsonFile<TItem> : ChangeableFile<TItem>
    {
        protected JsonSerializerSettings JsonSettings =
            new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()};

        protected ChangeableJsonFile(string path) : base(path)
        {
        }

        protected override string Serialize(List<TItem> items)
        {
            return JsonConvert.SerializeObject(items, JsonSettings);
        }

        protected override List<TItem> Deserialize(string content)
        {
            return JsonConvert.DeserializeObject<List<TItem>>(content, JsonSettings);
        }
    }
}