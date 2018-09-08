using System.Net;
using System.Net.Http;
using System.Security;
using System.Threading.Tasks;

namespace Orcus.Administration.Core.Clients
{
    public static class OrcusRestConnector
    {
        private static HttpClient _cachedHttpClient;

        public static async Task<OrcusRestClient> TryConnect(string username, SecureString password, IServerInfo serverInfo)
        {
            if (_cachedHttpClient == null)
                _cachedHttpClient =
                    new HttpClient(new HttpClientHandler
                    {
                        AutomaticDecompression = DecompressionMethods.GZip,
                    });

            _cachedHttpClient.BaseAddress = serverInfo.ServerUri;
            var client = new OrcusRestClient(username, password, _cachedHttpClient);
            await client.Initialize();
            return client;
        }
    }
}