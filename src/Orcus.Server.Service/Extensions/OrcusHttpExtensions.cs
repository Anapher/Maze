using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Orcus.Modules.Api;
using Orcus.Modules.Api.Request;
using Orcus.Modules.Api.Response;
using Orcus.Server.OrcusSockets.Internal;
using Orcus.Sockets.Internal;
using HttpHeaders = System.Net.Http.Headers.HttpHeaders;
using HttpMethod = System.Net.Http.HttpMethod;

namespace Orcus.Server.Service.Extensions
{
    public static class OrcusHttpExtensions
    {
        public static HttpRequestMessage ToHttpRequestMessage(this OrcusRequest request)
        {
            var builder = new UriBuilder {Path = request.Path, Query = request.QueryString.Value};

            var message = new HttpRequestMessage(new HttpMethod(request.Method), builder.Uri);
            request.Headers.CopyHeadersTo(message.Headers);
            message.Content = new RawStreamContent(request.Body);

            return message;
        }

        public static MessageOrcusRequest ToOrcusRequest(this HttpRequestMessage message) => new MessageOrcusRequest(message);

        public static HttpResponseMessage ToHttpResponseMessage(this OrcusResponse response)
        {
            var message = new HttpResponseMessage((HttpStatusCode) response.StatusCode);
            response.Headers.CopyHeadersTo(message.Headers);
            message.Content = new RawStreamContent(response.Body);

            return message;
        }

        public static HttpRequestMessage ToHttpRequestMessage(this HttpRequest httpRequest, string path)
        {
            var requestMessage =
                new HttpRequestMessage(new HttpMethod(httpRequest.Method),
                    new Uri("orcus://localhost/" + path, UriKind.Absolute))
                {
                    Content = new RawStreamContent(httpRequest.Body)
                };

            foreach (var header in httpRequest.Headers)
            {
                if (header.Key.Equals(HeaderNames.Authorization, StringComparison.OrdinalIgnoreCase))
                    continue;

                requestMessage.Headers.Add(header.Key, (IEnumerable<string>) header.Value);
            }

            return requestMessage;
        }

        public static void CopyToHttpResponse(this HttpResponseMessage responseMessage, HttpResponse httpResponse)
        {
            httpResponse.StatusCode = (int) responseMessage.StatusCode;
            responseMessage.Headers.CopyHeadersTo(httpResponse.Headers);
            httpResponse.Body = responseMessage.Content.AsStream();
        }

        public static Stream AsStream(this HttpContent content)
        {
            return ((RawStreamContent) content).Stream;
        }

        public static void CopyHeadersTo(this IHeaderDictionary headerDictionary, HttpHeaders httpHeaders)
        {
            foreach (var requestHeader in headerDictionary)
                httpHeaders.Add(requestHeader.Key, (IEnumerable<string>) requestHeader.Value);
        }

        public static void CopyHeadersTo(this HttpHeaders httpHeaders, IHeaderDictionary headerDictionary)
        {
            foreach (var httpHeader in httpHeaders)
                headerDictionary.Add(httpHeader.Key, new StringValues(httpHeader.Value.ToArray()));
        }
    }

    public class MessageOrcusRequest : OrcusRequest
    {
        private readonly HttpRequestMessage _requestMessage;

        public MessageOrcusRequest(HttpRequestMessage requestMessage)
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

        public override OrcusContext Context { get; set; }
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
            get => Headers["Content-Type"];
            set => Headers["Content-Type"] = value;
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