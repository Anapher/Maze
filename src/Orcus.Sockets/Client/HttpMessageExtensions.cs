using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Orcus.Sockets.Client.WebSocketSharp;
using Orcus.Sockets.Internal;

namespace Orcus.Sockets.Client
{
    public static class HttpMessageExtensions
    {
        public static async Task<string> GetHttpString(this HttpRequestMessage request)
        {
            var stringBuilder = new StringBuilder(64);

            stringBuilder.AppendFormat("{0} {1} HTTP/{2}", request.Method.Method, request.RequestUri, request.Version)
                .AppendLine();

            foreach (var requestHeader in request.Headers)
                stringBuilder
                    .AppendFormat("{0}: {1}", requestHeader.Key, new StringValues(requestHeader.Value.ToArray()))
                    .AppendLine();

            stringBuilder.AppendLine();

            if (request.Content != null)
            {
                var body = await request.Content.ReadAsStringAsync();
                stringBuilder.Append(body);
            }

            return stringBuilder.ToString();
        }

        public static async Task<HttpResponseMessage> DeserializeResponse(this Stream stream)
        {
            var response = new HttpResponseMessage();
            var headers = new HeaderDictionary();

            using (var streamReader = new StreamReader(stream, Encoding.UTF8, false, 8192, true))
            {
                var statusLine = await streamReader.ReadLineAsync();
                response.SetStatusProperties(statusLine);

                string line;
                while (!string.IsNullOrEmpty(line = await streamReader.ReadLineAsync()))
                {
                    var split = line.Split(new[] {": "}, 2, StringSplitOptions.None);
                    headers.Add(split[0], split[1]);
                }
            }

            var contentLength = headers.ContentLength;
            if (contentLength > 0)
            {
                var buffer = await stream.ReadBytes((int) contentLength.Value);
                response.Content = new ByteArrayPoolContent(buffer.Array, 0, buffer.Count);
            }

            foreach (var header in headers)
            {
                if (!HeadersHelper.IsContentHeader(header.Key)) //ignore content headers
                    response.Headers.Add(header.Key, (IEnumerable<string>) header.Value);
            }

            return response;
        }

        private static void SetStatusProperties(this HttpResponseMessage response, string line)
        {
            var parts = line.Split(new[] {' '}, 3);
            if (parts.Length != 3)
                throw new ArgumentException("Invalid status line", nameof(line));

            response.StatusCode = (HttpStatusCode) int.Parse(parts[1]);
            response.Version = Version.Parse(parts[0].Substring(5));
        }
    }
}