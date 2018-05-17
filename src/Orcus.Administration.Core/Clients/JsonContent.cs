using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Orcus.Administration.Core.Clients
{
    public class JsonContent : StringContent
    {
        public static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DateTimeZoneHandling = DateTimeZoneHandling.Utc
        };

        public JsonContent(object value) : base(JsonConvert.SerializeObject(value, JsonSettings), Encoding.UTF8, "application/json")
        {
        }
    }
}