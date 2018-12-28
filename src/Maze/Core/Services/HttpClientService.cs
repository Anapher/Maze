using System;
using System.Collections.Generic;
using System.Net.Http;
using Maze.Client.Library.Interfaces;
using Maze.Client.Library.Services;

namespace Maze.Core.Services
{
    public class HttpClientService : IHttpClientService, IDisposable
    {
        public HttpClientService(IEnumerable<IConfigureHttpMessageHandler> handlers)
        {
            HttpMessageHandler messageHandler = new HttpClientHandler();
            foreach (var handler in handlers)
                messageHandler = handler.Configure(messageHandler);

            Client = new HttpClient(messageHandler);
        }

        public HttpClient Client { get; }

        public void Dispose()
        {
            Client?.Dispose();
        }
    }
}