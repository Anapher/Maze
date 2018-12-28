using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Maze.Sockets.Internal;
using Maze.Sockets.Internal.Http;

namespace RequestTransmitter.Client.Utilities
{
    public static class HttpRequestSerializer
    {
        public static void FormatHeaders(HttpRequestMessage request, StreamWriter textWriter)
        {
            textWriter.Write(request.Method);
            textWriter.Write(" ");

            textWriter.WriteLine(request.RequestUri.ToString());

            EncodeHeaders(
                request.Headers.Concat(request.Content?.Headers ?? Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>())
                    .Select(x => new KeyValuePair<string, StringValues>(x.Key, x.Value.ToArray())),
                textWriter);
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

        public static async Task Format(HttpRequestMessage requestMessage, Stream targetStream)
        {
            using (var streamWriter = new StreamWriter(targetStream, Encoding.UTF8, 8192, leaveOpen: true))
            {
                FormatHeaders(requestMessage, streamWriter);
                streamWriter.WriteLine();
            }

            if (requestMessage.Content != null)
            {
                using (var contentStream = await requestMessage.Content.ReadAsStreamAsync())
                {
                    await contentStream.CopyToAsync(targetStream);
                }
            }
        }

        public static async Task<HttpRequestMessage> Decode(Stream sourceStream)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(8192);
            try
            {
                var length = await sourceStream.ReadAsync(buffer, 0, 8192);
                var bodyPosition = GetBodyPosition(new ArraySegment<byte>(buffer, 0, length));
                if (bodyPosition == -1)
                    throw new FormatException("The format of the http request seems invalid. CrLf,CrLf was not found in the first 8192 bytes.");

                var memoryStream = new MemoryStream();
                memoryStream.Write(buffer, bodyPosition, length - bodyPosition);
                memoryStream.Position = 0;

                var content = new StreamContent(new ConcatenatedStream(new[] {memoryStream, sourceStream}));

                using (var bodyMemoryStream = new MemoryStream(buffer, 0, bodyPosition, false))
                using (var streamReader = new StreamReader(bodyMemoryStream, Encoding.UTF8))
                {
                    return DecodeHeaders(content, streamReader);
                }
            }
            finally
            { 
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        private static HttpRequestMessage DecodeHeaders(HttpContent httpContent, StreamReader streamReader)
        {
            var request = new HttpRequestMessage();

            var firstLine = streamReader.ReadLine();
            var split = firstLine.Split(new[] { ' ' }, 2);
            request.Method = new HttpMethod(split[0]);
            request.RequestUri = new Uri(split[1], UriKind.RelativeOrAbsolute);

            var headers = new HeaderDictionary();
            DecodeHeaders(headers, streamReader);

            foreach (var header in headers)
            {
                if (HeadersHelper.IsContentHeader(header.Key))
                    httpContent.Headers.Add(header.Key, header.Value.ToList());
                else request.Headers.Add(header.Key, header.Value.ToList());
            }

            request.Content = httpContent;

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

        private static int GetBodyPosition(ArraySegment<byte> bytes)
        {
            var needle = new byte[] { 0x0D, 0x0A, 0x0D, 0x0A }; //crLF crLF
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
