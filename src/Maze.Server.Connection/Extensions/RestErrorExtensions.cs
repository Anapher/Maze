using System.ComponentModel.DataAnnotations;

namespace Maze.Server.Connection.Extensions
{
    public static class RestErrorExtensions
    {
        public static ValidationResult ValidateOnMember(this RestError error, params string[] memberNames)
        {
            return new RestErrorValidationResult(error, memberNames);
        }
    }
}