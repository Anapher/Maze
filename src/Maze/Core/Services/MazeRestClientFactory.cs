using System;
using Maze.Client.Library.Services;
using Maze.Core.Connection;

namespace Maze.Core.Services
{
    public interface IMazeRestClientFactory
    {
        MazeRestClient Create(Uri baseUri);
    }

    public class MazeRestClientFactory : IMazeRestClientFactory
    {
        private readonly IHttpClientService _httpClientService;

        public MazeRestClientFactory(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public MazeRestClient Create(Uri baseUri) => new MazeRestClient(_httpClientService.Client, baseUri);
    }
}