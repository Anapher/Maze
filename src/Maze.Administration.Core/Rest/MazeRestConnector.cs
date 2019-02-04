using System.Net;
using System.Net.Http;
using System.Security;
using System.Threading.Tasks;
using Maze.Administration.Library.Clients;

namespace Maze.Administration.Core.Rest
{
    public static class MazeRestConnector
    {
        public static async Task<MazeRestClient> TryConnect(string username, SecureString password, IServerInfo serverInfo)
        {
            var httpClient =
                new HttpClient(new HttpClientHandler {AutomaticDecompression = DecompressionMethods.GZip,}) {BaseAddress = serverInfo.ServerUri};

            var client = new MazeRestClient(username, serverInfo, password, httpClient);
            await client.Initialize();
            return client;
        }
    }
}