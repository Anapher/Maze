using System.Buffers;
using System.Net.Http;

namespace Orcus.Sockets.Client
{
    public class ByteArrayPoolContent : ByteArrayContent
    {
        private readonly byte[] _content;

        public ByteArrayPoolContent(byte[] content) : base(content)
        {
            _content = content;
        }

        public ByteArrayPoolContent(byte[] content, int offset, int count) : base(content, offset, count)
        {
            _content = content;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            ArrayPool<byte>.Shared.Return(_content);
        }
    }
}