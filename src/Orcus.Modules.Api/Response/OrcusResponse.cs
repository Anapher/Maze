using System;
using System.IO;
using System.Threading.Tasks;
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

    public static class OrcusResponseExtensions
    {
        private static readonly Func<object, Task> DisposeDelegate = disposable =>
        {
            ((IDisposable) disposable).Dispose();
            return Task.CompletedTask;
        };

        private static readonly Func<object, Task> CallbackDelegate = callback => ((Func<Task>) callback)();

        /// <summary>
        ///     Registers an object for disposal by the host once the request has finished processing.
        /// </summary>
        /// <param name="response">The <see cref="OrcusResponse" /></param>
        /// <param name="disposable">The object to be disposed.</param>
        public static void RegisterForDispose(this OrcusResponse response, IDisposable disposable)
        {
            response.OnCompleted(DisposeDelegate, disposable);
        }

        /// <summary>
        ///     Adds a delegate to be invoked just before response headers will be sent to the client.
        /// </summary>
        /// <param name="response">The <see cref="OrcusResponse" /></param>
        /// <param name="callback">The delegate to execute.</param>
        public static void OnStarting(this OrcusResponse response, Func<Task> callback)
        {
            response.OnStarting(CallbackDelegate, callback);
        }
    }
}