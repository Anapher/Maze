using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Maze.Administration.Library.Clients;
using Maze.Administration.Library.Exceptions;
using Maze.Server.Connection;

namespace Maze.Administration.TestingHelpers.Rest
{
    public class RestClientMock : IRestClient
    {
        public RestClientMock()
        {
            Requests = new List<MockRequest>();
        }

        Task<HttpResponseMessage> IRestClient.SendMessage(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            foreach (var mockRequest in Requests)
            {
                if (mockRequest.Comparers.All(x => x.Compare(request, mockRequest.Request)))
                {
                    return Task.FromResult(mockRequest.Response);
                }
            }

            throw new RestNotFoundException(new RestError());
        }

        public List<MockRequest> Requests { get; }
    }
}
