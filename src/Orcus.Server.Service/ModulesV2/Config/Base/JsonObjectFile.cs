using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Orcus.Server.Service.ModulesV2.Config.Base
{
    public abstract class JsonObjectFile<TObject> :SerializableObjectFile<TObject>
    {
        protected JsonSerializerSettings JsonSettings =
            new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

        protected JsonObjectFile(string path) : base(path)
        {
        }

        protected override string Serialize(TObject value)
        {
            return JsonConvert.SerializeObject(value, JsonSettings);
        }

        protected override TObject Deserialize(string content)
        {
            return JsonConvert.DeserializeObject<TObject>(content, JsonSettings);
        }
    }
}