using System;

namespace Tasks.Infrastructure.Core
{
    /// <summary>
    ///     An exception that occurred while parsing an <see cref="OrcusTask"/>
    /// </summary>
    public class TaskParsingException : Exception
    {
        /// <inheritdoc />
        public TaskParsingException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public TaskParsingException() : base()
        {
        }

        /// <inheritdoc />
        public TaskParsingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}