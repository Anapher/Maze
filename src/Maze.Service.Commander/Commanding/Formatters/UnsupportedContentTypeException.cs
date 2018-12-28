using System;

namespace Maze.Service.Commander.Commanding.Formatters
{
    /// <summary>
    ///     The <see cref="Exception" /> that is added to model state when a model binder for the body of the request is
    ///     unable to understand the request content type header.
    /// </summary>
    public class UnsupportedContentTypeException : Exception
    {
        /// <summary>
        ///     Creates a new instance of <see cref="UnsupportedContentTypeException" /> with the specified
        ///     exception <paramref name="message" />.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public UnsupportedContentTypeException(string message) : base(message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
        }
    }
}