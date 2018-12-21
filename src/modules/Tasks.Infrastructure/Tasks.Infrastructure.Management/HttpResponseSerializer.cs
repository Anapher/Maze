using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using Orcus.Sockets.Internal.Http;

namespace Tasks.Infrastructure.Management
{
    public static class HttpResponseSerializer
    {
        public static void FormatHeaders(HttpResponseMessage response, StreamWriter textWriter)
        {
            textWriter.WriteLine((int) response.StatusCode);

            EncodeHeaders(
                response.Headers.Concat(response.Content?.Headers ?? Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>())
                    .Select(x => new KeyValuePair<string, StringValues>(x.Key, x.Value.ToArray())), textWriter);
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

        public static async Task Format(HttpResponseMessage responseMessage, Stream targetStream)
        {
            using (var streamWriter = new StreamWriter(targetStream, Encoding.UTF8, 8192, true))
            {
                FormatHeaders(responseMessage, streamWriter);
                streamWriter.WriteLine();
            }

            if (responseMessage.Content != null)
                using (var contentStream = await responseMessage.Content.ReadAsStreamAsync())
                {
                    await contentStream.CopyToAsync(targetStream);
                }
        }
    }
}