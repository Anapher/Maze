using System.IO;
using Microsoft.AspNetCore.Http;
using Orcus.Modules.Api;
using Orcus.Modules.Api.Request;

namespace Orcus.Sockets.Internal.Http
{
    internal class DecodedOrcusRequest : OrcusRequest
    {
        public DecodedOrcusRequest()
        {
            Headers = new HeaderDictionary();
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
    }
}