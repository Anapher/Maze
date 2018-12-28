using System.ComponentModel.DataAnnotations;
using Maze.Server.Connection.Extensions;

namespace Maze.Server.Connection.Utilities
{
    public static class BusinessErrorHelper
    {
        public static ValidationResult FieldNullOrEmpty(string name)
        {
            return BusinessErrors.FieldNullOrEmpty(name).ValidateOnMember(name);
        }
    }
}