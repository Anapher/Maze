using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Maze.Modules.Api;
using Maze.Modules.Api.Request;
using Maze.Modules.Api.Response;
using Maze.Sockets.Internal;
using HttpHeaders = System.Net.Http.Headers.HttpHeaders;
using HttpMethod = System.Net.Http.HttpMethod;

namespace Maze.Server.Service.Extensions
{
    public static class MazeHttpExtensions
    {
        private static readonly Uri DummyBaseUri = new Uri("http://localhost/", UriKind.Absolute);

        public static HttpRequestMessage ToHttpRequestMessage(this MazeRequest request)
        {
            var builder = new UriBuilder {Path = request.Path, Query = request.QueryString.Value};

            var message =
                new HttpRequestMessage(new HttpMethod(request.Method), builder.Uri)
                {
                    Content = new RawStreamContent(request.Body)
                };

            request.Headers.CopyHeadersTo(message.Headers, message.Content.Headers);

            return message;
        }

        public static MessageMazeRequest ToMazeRequest(this HttpRequestMessage message) => new MessageMazeRequest(message);

        public static HttpResponseMessage ToHttpResponseMessage(this MazeResponse response)
        {
            var message =
                new HttpResponseMessage((HttpStatusCode) response.StatusCode)
                {
                    Content = new RawStreamContent(response.Body)
                };
            response.Headers.CopyHeadersTo(message.Headers, message.Content.Headers);

            return message;
        }

        public static HttpRequestMessage ToHttpRequestMessage(this HttpRequest httpRequest, string path)
        {
            var requestMessage =
                new HttpRequestMessage(new HttpMethod(httpRequest.Method),
                    new Uri(DummyBaseUri, path + httpRequest.QueryString))
                {
                    Content = new RawStreamContent(httpRequest.Body)
                };

            foreach (var header in httpRequest.Headers)
            {
                if (header.Key.Equals(HeaderNames.Authorization, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (HeadersHelper.IsContentHeader(header.Key))
                    requestMessage.Content.Headers.Add(header.Key, (IEnumerable<string>) header.Value);
                else
                    requestMessage.Headers.Add(header.Key, (IEnumerable<string>) header.Value);
            }

            return requestMessage;
        }

        public static async Task CopyToHttpResponse(this HttpResponseMessage responseMessage, HttpResponse httpResponse)
        {
            httpResponse.StatusCode = (int) responseMessage.StatusCode;
            responseMessage.Headers.CopyHeadersTo(httpResponse.Headers);

            if (responseMessage.Content != null)
            {
                responseMessage.Content.Headers.CopyHeadersTo(httpResponse.Headers);
                await responseMessage.Content.AsStream().CopyToAsync(httpResponse.Body);
            }
        }

        public static Stream AsStream(this HttpContent content)
        {
            return ((RawStreamContent) content).Stream;
        }

        public static void CopyHeadersTo(this IHeaderDictionary headerDictionary, HttpHeaders httpHeaders,
            HttpContentHeaders contentHeaders)
        {
            foreach (var requestHeader in headerDictionary)
                if (HeadersHelper.IsContentHeader(requestHeader.Key))
                    contentHeaders.Add(requestHeader.Key, (IEnumerable<string>) requestHeader.Value);
                else
                    httpHeaders.Add(requestHeader.Key, (IEnumerable<string>) requestHeader.Value);
        }

        public static void CopyHeadersTo(this HttpHeaders httpHeaders, IHeaderDictionary headerDictionary)
        {
            foreach (var httpHeader in httpHeaders)
                headerDictionary.Add(httpHeader.Key, new StringValues(httpHeader.Value.ToArray()));
        }
    }

    public class MessageMazeRequest : MazeRequest
    {
        private readonly HttpRequestMessage _requestMessage;

        public MessageMazeRequest(HttpRequestMessage requestMessage)
        {
            _requestMessage = requestMessage;
            Method = requestMessage.Method.Method;
            Path = requestMessage.RequestUri.AbsolutePath;
            QueryString = new QueryString(requestMessage.RequestUri.Query);

            Headers = new HeaderDictionary(
                requestMessage.Headers.ToDictionary(x => x.Key, x => new StringValues(x.Value.ToArray())));

            var queryCollection = requestMessage.RequestUri.ParseQueryString();
            Query = new QueryCollection(queryCollection.AllKeys.ToDictionary(x => x,
                x => new StringValues(queryCollection.GetValues(x))));
        }

        public override MazeContext Context { get; set; }
        public override string Method { get; set; }
        public override PathString Path { get; set; }
        public override QueryString QueryString { get; set; }
        public override IQueryCollection Query { get; set; }
        public override IHeaderDictionary Headers { get; }

        public override long? ContentLength
        {
            get => Headers.ContentLength;
            set => Headers.ContentLength = value;
        }

        public override string ContentType
        {
            get => Headers[HeaderNames.ContentType];
            set => Headers[HeaderNames.ContentType] = value;
        }

        public override Stream Body { get; set; }

        public async Task InitializeBody()
        {
            if (_requestMessage.Content is RawStreamContent rawStreamContent)
                Body = rawStreamContent.Stream;
            else
                Body = await _requestMessage.Content.ReadAsStreamAsync();
        }
    }
}