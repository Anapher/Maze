using System.Net;
using System.Net.Http;
using System.Security;
using System.Threading.Tasks;

namespace Maze.Administration.Core.Rest
{
    public static class MazeRestConnector
    {
        public static async Task<MazeRestClient> TryConnect(string username, SecureString password, IServerInfo serverInfo)
        {
            var httpClient =
                new HttpClient(new HttpClientHandler {AutomaticDecompression = DecompressionMethods.GZip,}) {BaseAddress = serverInfo.ServerUri};

            var client = new MazeRestClient(username, password, httpClient);
            await client.Initialize();
            return client;
        }
    }
}