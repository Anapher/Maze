using System.IO;
using Microsoft.AspNetCore.Http;

namespace Orcus.Modules.Api.Response
{
    /// <summary>
    ///     Represents the outgoing side of an individual Orcus request.
    /// </summary>
    public abstract class OrcusResponse
    {
        /// <summary>Gets or sets the Orcus response code.</summary>
        public abstract int StatusCode { get; set; }

        /// <summary>Gets the response headers.</summary>
        public abstract IHeaderDictionary Headers { get; }

        /// <summary>
        ///     Gets or sets the response body <see cref="T:System.IO.Stream" />.
        /// </summary>
        public abstract Stream Body { get; set; }

        /// <summary>
        ///     Gets or sets the value for the <c>Content-Length</c> response header.
        /// </summary>
        public abstract long? ContentLength { get; set; }

        /// <summary>
        ///     Gets or sets the value for the <c>Content-Type</c> response header.
        /// </summary>
        public abstract string ContentType { get; set; }
    }
}