using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Orcus.Sockets.Internal
{
    public class RawStreamContent : HttpContent
    {
        private readonly bool _leaveOpen;

        public RawStreamContent(Stream stream, bool leaveOpen)
        {
            _leaveOpen = leaveOpen;
            Stream = stream;
        }

        public RawStreamContent(Stream stream) : this(stream, false)
        {
        }

        public Stream Stream { get; }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context) =>
            throw new NotSupportedException();

        protected override bool TryComputeLength(out long length)
        {
            length = Stream.Length;
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_leaveOpen)
                Stream.Dispose();

            base.Dispose(disposing);
        }
    }
}