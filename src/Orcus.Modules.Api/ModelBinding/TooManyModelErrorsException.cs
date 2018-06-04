using System;

namespace Orcus.Modules.Api.ModelBinding
{
    /// <summary>
    ///     The <see cref="Exception" /> that is thrown when too many model errors are encountered.
    /// </summary>
    public class TooManyModelErrorsException : Exception
    {
        /// <summary>
        ///     Creates a new instance of <see cref="TooManyModelErrorsException" /> with the specified
        ///     exception <paramref name="message" />.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public TooManyModelErrorsException(string message)
            : base(message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
        }
    }
}