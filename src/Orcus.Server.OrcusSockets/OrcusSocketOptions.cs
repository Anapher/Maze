using System;

namespace Orcus.Server.OrcusSockets
{
    public class OrcusSocketOptions
    {
        /// <summary>
        ///     Gets or sets the frequency at which to send Ping/Pong keep-alive control frames.
        ///     The default is two minutes.
        /// </summary>
        public TimeSpan KeepAliveInterval { get; set; }

        /// <summary>
        ///     Gets or sets the size of the protocol buffer used to receive and parse frames.
        ///     The default is 4kb.
        /// </summary>
        public int ReceiveBufferSize { get; set; }
    }
}