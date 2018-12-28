using System.IO;
using Microsoft.AspNetCore.Http;

namespace Maze.Modules.Api.Request
{
    /// <summary>
    ///     Represents the incoming side of an individual Maze request.
    /// </summary>
    public abstract class MazeRequest
    {
        public abstract MazeContext Context { get; set; }

        /// <summary>Gets or sets the Maze method.</summary>
        /// <returns>The HTTP method.</returns>
        public abstract string Method { get; set; }

        /// <summary>Gets or sets the request path from RequestPath.</summary>
        /// <returns>The request path from RequestPath.</returns>
        public abstract PathString Path { get; set; }

        /// <summary>
        ///     Gets or sets the raw query string used to create the query collection in Request.Query.
        /// </summary>
        /// <returns>The raw query string.</returns>
        public abstract QueryString QueryString { get; set; }

        /// <summary>
        ///     Gets the query value collection parsed from Request.QueryString.
        /// </summary>
        /// <returns>The query value collection parsed from Request.QueryString.</returns>
        public abstract IQueryCollection Query { get; set; }

        /// <summary>Gets the request headers.</summary>
        /// <returns>The request headers.</returns>
        public abstract IHeaderDictionary Headers { get; }

        /// <summary>Gets or sets the Content-Length header.</summary>
        /// <returns>The value of the Content-Length header, if any.</returns>
        public abstract long? ContentLength { get; set; }

        /// <summary>Gets or sets the Content-Type header.</summary>
        /// <returns>The Content-Type header.</returns>
        public abstract string ContentType { get; set; }

        /// <summary>Gets or sets the RequestBody Stream.</summary>
        /// <returns>The RequestBody Stream.</returns>
        public abstract Stream Body { get; set; }
    }
}