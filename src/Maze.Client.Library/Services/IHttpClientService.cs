using System.Net.Http;

namespace Maze.Client.Library.Services
{
    /// <summary>
    ///     A shared HttpClient
    /// </summary>
    public interface IHttpClientService
    {
        /// <summary>
        /// Gets a server-shared http client to make http requests
        /// </summary>
        HttpClient Client { get; }
    }
}