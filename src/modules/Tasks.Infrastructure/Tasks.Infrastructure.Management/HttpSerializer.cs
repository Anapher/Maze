using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Orcus.Sockets.Internal.Http;

namespace Tasks.Infrastructure.Management
{
    public static class HttpSerializer
    {
        public static StringBuilder FormatHeaders(HttpResponseMessage responseMessage)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(((int) responseMessage.StatusCode).ToString());
            foreach (var header in responseMessage.Headers)
            {
                stringBuilder.Append(header.Key);
                stringBuilder.Append(": ");
                stringBuilder.AppendLine(StringValuesExtensions.FormatStringValues(new StringValues(header.Value.ToArray())));
            }

            return stringBuilder;
        }

        public static async Task Format(HttpResponseMessage responseMessage, Stream targetStream)
        {
            using (var streamWriter = new StreamWriter(targetStream, Encoding.UTF8))
            {
                var sb = FormatHeaders(responseMessage);
                streamWriter.Write(sb.ToString());
                streamWriter.WriteLine();

                await responseMessage.Content.CopyToAsync(targetStream);
            }
        }
    }
}
