using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Maze.Modules.Api.Response
{
    /// <summary>
    ///     Represents the outgoing side of an individual Maze request.
    /// </summary>
    public abstract class MazeResponse
    {
        /// <summary>Gets or sets the Maze response code.</summary>
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

        /// <summary>
        ///     Gets a value indicating whether response headers have been sent to the client.
        /// </summary>
        public abstract bool HasStarted { get; }

        /// <summary>
        ///     Adds a delegate to be invoked just before response headers will be sent to the client.
        /// </summary>
        /// <param name="callback">The delegate to execute.</param>
        /// <param name="state">A state object to capture and pass back to the delegate.</param>
        public abstract void OnStarting(Func<object, Task> callback, object state);

        /// <summary>
        ///     Adds a delegate to be invoked after the response has finished being sent to the client.
        /// </summary>
        /// <param name="callback">The delegate to invoke.</param>
        /// <param name="state">A state object to capture and pass back to the delegate.</param>
        public abstract void OnCompleted(Func<object, Task> callback, object state);
    }
}