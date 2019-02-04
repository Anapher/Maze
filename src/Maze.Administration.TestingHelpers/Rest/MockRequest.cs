using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Maze.Administration.TestingHelpers.Rest.Comparers;

namespace Maze.Administration.TestingHelpers.Rest
{
    public class MockRequest
    {
        public MockRequest(HttpRequestMessage message)
        {
            Request = message;
            Response = new HttpResponseMessage(HttpStatusCode.OK);

            Comparers = new List<IRequestComparer> {new HttpMethodComparer(), new RouteComparer(), new QueryComparer(), new HttpBodyComparer()};

        }

        public HttpRequestMessage Request { get; }
        public HttpResponseMessage Response { get; }

        public List<IRequestComparer> Comparers { get; }
    }
}