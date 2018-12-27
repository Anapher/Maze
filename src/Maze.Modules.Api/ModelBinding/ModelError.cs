using System;

namespace Orcus.Modules.Api.ModelBinding
{
    public class ModelError
    {
        public ModelError(Exception exception)
            : this(exception, null)
        {
            if (exception == null) throw new ArgumentNullException(nameof(exception));
        }

        public ModelError(Exception exception, string errorMessage)
            : this(errorMessage)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }

        public ModelError(string errorMessage)
        {
            ErrorMessage = errorMessage ?? string.Empty;
        }

        public Exception Exception { get; }

        public string ErrorMessage { get; }
    }
}