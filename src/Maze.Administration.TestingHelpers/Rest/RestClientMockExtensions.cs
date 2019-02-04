using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Maze.Administration.Library.Clients;

namespace Maze.Administration.TestingHelpers.Rest
{
    public static class RestClientMockExtensions
    {
        public static RestClientMock AddResource(this RestClientMock restClientMock, Func<IRestClient, Task> requestResourceAction)
        {
            var interceptingRestClient = new InterceptingRestClient();

            try
            {
                requestResourceAction(interceptingRestClient).Wait();
            }
            catch (ThisIsJustAnHttpClientMockException)
            {
            }

            if (interceptingRestClient.Request == null)
                throw new InvalidOperationException("The resource did not call the rest client.");

            var mockRequest = new MockRequest(interceptingRestClient.Request);
            restClientMock.Requests.Add(mockRequest);

            return restClientMock;
        }

        private class InterceptingRestClient : IRestClient
        {
            public HttpRequestMessage Request { get; private set; }

            public Task<HttpResponseMessage> SendMessage(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Request = request;
                throw new ThisIsJustAnHttpClientMockException();
            }
        }

        private class ThisIsJustAnHttpClientMockException : Exception { }
    }
}