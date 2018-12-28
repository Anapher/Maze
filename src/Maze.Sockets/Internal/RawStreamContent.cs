using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Maze.Sockets.Internal
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

        protected override Task<Stream> CreateContentReadStreamAsync() => Task.FromResult(Stream);

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context) =>
            Stream.CopyToAsync(stream);

        protected override bool TryComputeLength(out long length)
        {
            length = 0;
            return false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_leaveOpen)
                Stream.Dispose();

            base.Dispose(disposing);
        }
    }
}