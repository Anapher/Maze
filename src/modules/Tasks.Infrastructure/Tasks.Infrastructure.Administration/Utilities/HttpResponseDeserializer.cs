using System;
using System.Collections.Generic;
using System.Net.Http;
using Maze.Sockets.Internal.Http;

namespace Tasks.Infrastructure.Administration.Utilities
{
    public static class HttpResponseDeserializer
    {
        public static HttpResponseMessage DecodeResponse(string commandResultBase64)
        {
            var binary = Convert.FromBase64String(commandResultBase64);

            var length = HttpFormatter.ParseResponse(new ArraySegment<byte>(binary), out var response, out var contentHeaders);
            if (binary.Length > length)
            {
                response.Content = new ByteArrayContent(binary, length, binary.Length - length);
                foreach (var contentHeader in contentHeaders)
                    response.Content.Headers.Add(contentHeader.Key, (IEnumerable<string>) contentHeader.Value);
            }

            return response;
        }
    }
}