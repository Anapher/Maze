using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Maze.Client.Library.Clients;
using Maze.Client.Library.Exceptions;
using Maze.Client.Library.Services;

namespace Maze.Core.Connection
{
    public class MazeRestClientWrapper : IRestClient, IMazeRestClient
    {
        private IMazeRestClient _restClient;

        public Uri BaseUri
        {
            get
            {
                if (_restClient == null)
                    throw new RestClientNotConnectedException();

                return _restClient.BaseUri;
            }
        }

        public string Jwt
        {
            get
            {
                if (_restClient == null)
                    throw new RestClientNotConnectedException();

                return _restClient.Jwt;
            }
        }

        public Task<HttpResponseMessage> SendMessage(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_restClient == null)
                throw new RestClientNotConnectedException();

            return _restClient.SendMessage(request, cancellationToken);
        }

        public void Initialize(IMazeRestClient restClient)
        {
            _restClient = restClient;
        }
    }
}