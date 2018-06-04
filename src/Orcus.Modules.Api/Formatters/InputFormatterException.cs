using System;

namespace Orcus.Modules.Api.Formatters
{
    /// <summary>
    ///     Exception thrown by <see cref="IInputFormatter" /> when the input is not in an expected format.
    /// </summary>
    public class InputFormatterException : Exception
    {
        public InputFormatterException()
        {
        }

        public InputFormatterException(string message)
            : base(message)
        {
        }

        public InputFormatterException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}