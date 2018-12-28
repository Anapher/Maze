using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Maze.Server.Service.Modules.Config.Base
{
    public abstract class JsonObjectFile<TObject> : SerializableObjectFile<TObject>
    {
        protected JsonSerializerSettings JsonSettings =
            new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()};

        protected JsonObjectFile(string path) : base(path)
        {
        }

        protected override string Serialize(TObject value) =>
            JsonConvert.SerializeObject(value, Formatting.Indented, JsonSettings);

        protected override TObject Deserialize(string content) =>
            JsonConvert.DeserializeObject<TObject>(content, JsonSettings);
    }
}