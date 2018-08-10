using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Orcus.Modules.Api.Request;
using Orcus.Modules.Api.Response;

namespace Orcus.Sockets.Internal.Http
{
    //GET /wiki/Spezial:Search? search = Katzen & go = Artikel HTTP/1.1
    //Host: de.wikipedia.org
    public static class HttpFormatter
    {
        private static readonly UTF8Encoding Encoding = new UTF8Encoding(false);

        public static int FormatRequest(HttpRequestMessage request, ArraySegment<byte> buffer)
        {
            var memoryStream = new MemoryStream(buffer.Array, buffer.Offset, buffer.Count, true);
            var streamWriter = new StreamWriter(memoryStream, Encoding);
            EncodeRequest(request, streamWriter);
            streamWriter.Flush();

            return (int) memoryStream.Position;
        }

        public static int ParseRequest(ArraySegment<byte> buffer, out OrcusRequest request)
        {
            var memoryStream = new MemoryStream(buffer.Array, buffer.Offset, buffer.Count, false);
            var streamReader = new StreamReader(memoryStream, Encoding);
            request = DecodeRequest(streamReader);
            return (int) memoryStream.Position;
        }

        public static int FormatResponse(OrcusResponse orcusResponse, ArraySegment<byte> buffer)
        {
            var memoryStream = new MemoryStream(buffer.Array, buffer.Offset, buffer.Count, true);
            var streamWriter = new StreamWriter(memoryStream, Encoding);
            EncodeResponse(orcusResponse, streamWriter);
            streamWriter.Flush();

            return (int) memoryStream.Position;
        }

        public static int ParseResponse(ArraySegment<byte> buffer, out HttpResponseMessage response)
        {
            var memoryStream = new MemoryStream(buffer.Array, buffer.Offset, buffer.Count, false);
            var streamReader = new StreamReader(memoryStream, Encoding);
            response = DecodeResponse(streamReader);
            return (int) memoryStream.Position;
        }

        private static void EncodeRequest(HttpRequestMessage request, TextWriter textWriter)
        {
            textWriter.Write(request.Method);
            textWriter.Write(" ");

            textWriter.WriteLine(request.RequestUri.PathAndQuery);

            EncodeHeaders(request.Headers.Select(x => new KeyValuePair<string, StringValues>(x.Key, x.Value.ToArray())),
                textWriter);

            textWriter.WriteLine(); //finish
        }

        private static OrcusRequest DecodeRequest(TextReader textReader)
        {
            var request = new DecodedOrcusRequest();

            var requestLine = textReader.ReadLine();
            var delimiterIndex = requestLine.IndexOf(' ');

            request.Method = requestLine.Substring(0, delimiterIndex);
            var uri = new Uri("orcus://localhost" + requestLine.Substring(delimiterIndex + 1, requestLine.Length - delimiterIndex - 1), UriKind.Absolute);
            request.Path = PathString.FromUriComponent(uri.LocalPath);
            request.QueryString = QueryString.FromUriComponent(uri);
            request.Query = new QueryCollection(QueryHelpers.ParseNullableQuery(uri.Query));

            DecodeHeaders(request.Headers, textReader);
            
            return request;
        }

        private static void DecodeHeaders(IHeaderDictionary headers, TextReader textReader)
        {
            string line;
            while (!string.IsNullOrEmpty(line = textReader.ReadLine()))
            {
                var keyEnd = line.IndexOf(": ");
                var key = line.Substring(0, keyEnd);
                var values = line.Substring(keyEnd + 2, line.Length - 2 - keyEnd);

                headers.AppendCommaSeparatedValues(key, values);
            }
        }

        private static void EncodeHeaders(IEnumerable<KeyValuePair<string, StringValues>> headers, TextWriter textWriter)
        {
            foreach (var header in headers)
            {
                textWriter.Write(header.Key);
                textWriter.Write(": ");
                textWriter.WriteLine(StringValuesExtensions.FormatStringValues(header.Value));
            }
        }

        private static void EncodeResponse(OrcusResponse response, TextWriter textWriter)
        {
            textWriter.WriteLine(response.StatusCode);

            EncodeHeaders(response.Headers, textWriter);

            textWriter.WriteLine(); //finish
        }

        private static HttpResponseMessage DecodeResponse(TextReader textReader)
        {
            var response = new HttpResponseMessage();
            var statusCode = int.Parse(textReader.ReadLine());
            response.StatusCode = (HttpStatusCode) statusCode;

            var headers = new HeaderDictionary();
            DecodeHeaders(headers, textReader);
            foreach (var header in headers)
                response.Headers.Add(header.Key, (string[]) header.Value);

            return response;
        }
    }
}