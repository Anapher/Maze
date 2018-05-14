using System.Collections.Generic;
using System.IO;

namespace Orcus.Modules.Api.Response
{
    public class OrcusResponse
    {
        public Stream Body { get; set; }
        public long? ContentLength { get; set; }
        public string ContentType { get; set; }
        public int StatusCode { get; set; }
        public IDictionary<string, string> Headers { get; set; }
    }
}