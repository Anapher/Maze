using System.Net;
using System.Net.Http;
using System.Security;
using System.Threading.Tasks;

namespace Maze.Administration.Core.Rest
{
    public static class MazeRestConnector
    {
        private static HttpClient _cachedHttpClient;

        public static async Task<MazeRestClient> TryConnect(string username, SecureString password, IServerInfo serverInfo)
        {
            if (_cachedHttpClient == null)
                _cachedHttpClient =
                    new HttpClient(new HttpClientHandler
                    {
                        AutomaticDecompression = DecompressionMethods.GZip,
                    });

            _cachedHttpClient.BaseAddress = serverInfo.ServerUri;
            var client = new MazeRestClient(username, password, _cachedHttpClient);
            await client.Initialize();
            return client;
        }
    }
}