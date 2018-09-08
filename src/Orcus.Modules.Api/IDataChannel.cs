using System;

namespace Orcus.Modules.Api
{
    public interface IDataChannel : IDisposable
    {
        /// <summary>
        ///     Reserve bytes at the beginning of the <see cref="Send" /> buffer for custom headers
        /// </summary>
        int RequiredOffset { get; set; }

        /// <summary>
        ///     The delegate which will get invoked when a package should be sent to the remote site.
        /// </summary>
        SendDelegate Send { get; set; }

        /// <summary>
        ///     Receive data
        /// </summary>
        /// <param name="buffer">The array of bytes</param>
        /// <param name="offset">The offset where the data begins</param>
        /// <param name="count">The amount of bytes which were received</param>
        void ReceiveData(byte[] buffer, int offset, int count);
    }
}