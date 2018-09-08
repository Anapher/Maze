using System;
using System.Net.Http;
using Orcus.Modules.Api;

namespace Orcus.Administration.Library.Channels
{
    /// <summary>
    ///     A data channel that controls it's own underlying lifespan, it can close itself. The event
    ///     <see cref="CloseChannel" /> will close the channel.
    /// </summary>
    public interface IAwareDataChannel : IDataChannel
    {
        /// <summary>
        ///     The event that should be fired once the channel is no longer needed (e. g. in the
        ///     <see cref="IDisposable.Dispose" /> method)
        /// </summary>
        event EventHandler CloseChannel;

        /// <summary>
        ///     Initialize the data channel after it was activated
        /// </summary>
        /// <param name="responseMessage">The response message that created this channel</param>
        void Initialize(HttpResponseMessage responseMessage);
    }
}