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

            using (var streamWriter = new StreamWriter(memoryStream, Encoding, 8192, true))
                EncodeRequest(request, streamWriter);
            
            return (int) memoryStream.Position;
        }

        public static int ParseRequest(ArraySegment<byte> buffer, out OrcusRequest request)
        {
            var memoryStream = new MemoryStream(buffer.Array, buffer.Offset, buffer.Count, false);
            var streamReader = new StreamReader(memoryStream, Encoding);
            request = DecodeRequest(streamReader);

            return GetBodyPosition(buffer) - buffer.Offset;
        }

        public static int FormatResponse(OrcusResponse orcusResponse, ArraySegment<byte> buffer)
        {
            var memoryStream = new MemoryStream(buffer.Array, buffer.Offset, buffer.Count, true);

            using (var streamWriter = new StreamWriter(memoryStream, Encoding, 8192, true))
                EncodeResponse(orcusResponse, streamWriter);

            return (int) memoryStream.Position;
        }

        public static int ParseResponse(ArraySegment<byte> buffer, out HttpResponseMessage response, out IHeaderDictionary contentHeaders)
        {
            var memoryStream = new MemoryStream(buffer.Array, buffer.Offset, buffer.Count, false);
            var streamReader = new StreamReader(memoryStream, Encoding);
            (response, contentHeaders) = DecodeResponse(streamReader);

            return GetBodyPosition(buffer) - buffer.Offset;
        }

        private static void EncodeRequest(HttpRequestMessage request, TextWriter textWriter)
        {
            textWriter.Write(request.Method);
            textWriter.Write(" ");

            if (request.RequestUri.IsAbsoluteUri)
                textWriter.WriteLine(request.RequestUri.PathAndQuery);
            else
            {
                var relativeUri = request.RequestUri.ToString();
                if (!relativeUri.StartsWith("/"))
                    relativeUri = "/" + relativeUri;
                textWriter.WriteLine(relativeUri);
            }

            EncodeHeaders(request.Headers.Select(x => new KeyValuePair<string, StringValues>(x.Key, x.Value.ToArray())),
                textWriter);
            if (request.Content != null)
            {
                var foo = request.Content.Headers.ContentLength;
                EncodeHeaders(request.Content.Headers.Select(
                        x => new KeyValuePair<string, StringValues>(x.Key, x.Value.ToArray())), textWriter);}

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

                if (values[0] == '"')
                    headers.AppendCommaSeparatedValues(key, values);
                else
                    headers.Add(key, values);
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

        private static (HttpResponseMessage, IHeaderDictionary) DecodeResponse(TextReader textReader)
        {
            var response = new HttpResponseMessage();
            var contentHeaders = new HeaderDictionary();

            var line = textReader.ReadLine();
            var statusCode = int.Parse(line);
            response.StatusCode = (HttpStatusCode) statusCode;

            var headers = new HeaderDictionary();
            DecodeHeaders(headers, textReader);
            foreach (var header in headers)
            {
                if (HeadersHelper.IsContentHeader(header.Key))
                    contentHeaders.Add(header);
                else
                    response.Headers.Add(header.Key, (string[]) header.Value);
            }

            return (response, contentHeaders);
        }

        private static int GetBodyPosition(ArraySegment<byte> bytes)
        {
            var needle = new byte[] {0x0D, 0x0A, 0x0D, 0x0A}; //crLF crLF
            return SearchBytes(bytes, needle) + 4;
        }

        //https://stackoverflow.com/a/26880541
        private static int SearchBytes(ArraySegment<byte> haystack, byte[] needle)
        {
            var len = needle.Length;
            var limit = haystack.Count - len + haystack.Offset;
            for (var i = haystack.Offset; i <= limit; i++)
            {
                var k = 0;
                for (; k < len; k++)
                {
                    if (needle[k] != haystack.Array[i + k]) break;
                }
                if (k == len) return i;
            }
            return -1;
        }
    }
}