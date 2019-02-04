using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Maze.Administration.Library.Clients;
using Maze.Administration.TestingHelpers.Rest.Comparers;

namespace Maze.Administration.TestingHelpers.Rest
{
    public static class MockRequestExtensions
    {
        public static MockRequest AcceptAnyBody(this MockRequest mockRequest)
        {
            return mockRequest.RemoveComparer<HttpBodyComparer>();
        }

        public static MockRequest AcceptAnyQuery(this MockRequest mockRequest)
        {
            return mockRequest.RemoveComparer<QueryComparer>();
        }

        public static MockRequest RemoveComparer<TComparer>(this MockRequest mockRequest) where TComparer : IRequestComparer
        {
            var bodyComparer = mockRequest.Comparers.OfType<TComparer>().FirstOrDefault();
            if (bodyComparer != null)
                mockRequest.Comparers.Remove(bodyComparer);

            return mockRequest;
        }

        public static MockRequest ResponseJsonObject(this MockRequest mockRequest, object data)
        {
            mockRequest.Response.Content = new JsonContent(data);
            return mockRequest;
        }

        public static MockRequest ResponseContent(this MockRequest mockRequest, HttpContent httpContent)
        {
            mockRequest.Response.Content = httpContent;
            return mockRequest;
        }

        public static MockRequest ChangeResponseHeader(this MockRequest mockRequest, Action<HttpResponseHeaders> addHeaders)
        {
            addHeaders(mockRequest.Response.Headers);
            return mockRequest;
        }

        public static MockRequest StatusCode(this MockRequest mockRequest, HttpStatusCode statusCode)
        {
            mockRequest.Response.StatusCode = statusCode;
            return mockRequest;
        }
    }
}