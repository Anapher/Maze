using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using Orcus.Modules.Api.Request;

namespace Orcus.Server.OrcusSockets.Internal.Http
{
    //GET /wiki/Spezial:Search? search = Katzen & go = Artikel HTTP/1.1
    //Host: de.wikipedia.org
    public static class HttpFormatter
    {
        public static int FormatRequest(OrcusRequest orcusRequest, ArraySegment<byte> buffer)
        {
            var memoryStream = new MemoryStream(buffer.Array, buffer.Offset, buffer.Count, true);
            var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8);
            EncodeRequest(orcusRequest, streamWriter);
            streamWriter.Flush();

            return (int) memoryStream.Position;
        }

        public static int ParseRequest(ArraySegment<byte> buffer, out OrcusRequest request)
        {
            var memoryStream = new MemoryStream(buffer.Array, buffer.Offset, buffer.Count, false);
            var streamReader = new StreamReader(memoryStream, Encoding.UTF8);
            request = DecodeRequest(streamReader);
            return (int) memoryStream.Position;
        }

        public static void EncodeRequest(OrcusRequest request, TextWriter textWriter)
        {
            textWriter.Write(request.Method);
            textWriter.Write(" ");

            var uriBuilder = new UriBuilder
            {
                Path = request.Path.ToUriComponent(),
                Query = request.QueryString.ToUriComponent()
            };

            textWriter.WriteLine(uriBuilder.Uri.ToString());

            foreach (var header in request.Headers)
            {
                textWriter.Write(header.Key);
                textWriter.Write(": ");
                textWriter.WriteLine(request.Headers.GetCommaSeparatedValues(header.Key));
            }

            textWriter.WriteLine(); //finish
        }

        public static OrcusRequest DecodeRequest(TextReader textReader)
        {
            var request = new DecodedOrcusRequest();

            var requestLine = textReader.ReadLine();
            var delimiterIndex = requestLine.IndexOf(' ');

            request.Method = requestLine.Substring(0, delimiterIndex);
            var uri = new Uri(requestLine.Substring(delimiterIndex + 1, requestLine.Length - delimiterIndex - 1));
            request.Path = PathString.FromUriComponent(uri);
            request.QueryString = QueryString.FromUriComponent(uri);

            var header = new HeaderDictionary();

            string line;
            while ((line = textReader.ReadLine()) != null)
            {
                var keyEnd = line.IndexOf(": ");
                var key = line.Substring(0, keyEnd);
                var values = line.Substring(keyEnd + 2, line.Length - 2 - keyEnd);

                header.AppendCommaSeparatedValues(key, values);
            }

            return request;
        }
    }

    internal class DecodedOrcusRequest : OrcusRequest
    {
        public override string Method { get; set; }
        public override PathString Path { get; set; }
        public override QueryString QueryString { get; set; }
        public override IQueryCollection Query { get; set; }
        public override IHeaderDictionary Headers { get; set; }

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
    }
}