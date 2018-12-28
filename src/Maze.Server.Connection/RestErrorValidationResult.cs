using System.ComponentModel.DataAnnotations;

namespace Maze.Server.Connection
{
    public class RestErrorValidationResult : ValidationResult
    {
        public RestErrorValidationResult(RestError restError) : base(restError.Message)
        {
            Error = restError;
        }

        public RestErrorValidationResult(RestError restError, params string[] memberNames) : base(restError.Message,
            memberNames)
        {
            Error = restError;
        }

        public RestError Error { get; }

        public static implicit operator RestErrorValidationResult(RestError restError)
        {
            return new RestErrorValidationResult(restError);
        }
    }
}