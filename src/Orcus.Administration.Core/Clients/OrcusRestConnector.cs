using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Orcus.Administration.Core.Clients
{
    public static class OrcusRestConnector
    {
        private static HttpClient _cachedHttpClient;

        public static async Task<IOrcusRestClient> TryConnect(string username, SecureString password, IServerInfo serverInfo)
        {
            if (_cachedHttpClient == null)
                _cachedHttpClient = new HttpClient();

            _cachedHttpClient.BaseAddress = serverInfo.ServerUri;
            var client = new OrcusRestClient(username, password, _cachedHttpClient);
            await client.Initialize();
            return client;
        }
    }

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