using System.ComponentModel.DataAnnotations;
using Orcus.Server.Connection.Extensions;

namespace Orcus.Server.Connection.Utilities
{
    public static class BusinessErrorHelper
    {
        public static ValidationResult FieldNullOrEmpty(string name)
        {
            return BusinessErrors.FieldNullOrEmpty(name).ValidateOnMember(name);
        }
    }
}