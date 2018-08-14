using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Orcus.Server.Connection.Authentication
{
    public class LoginInfo : IValidatableObject
    {
        [Required] public string Username { get; set; }
        [Required] public string Password { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Username))
                yield return BusinessErrors.FieldNullOrEmpty(nameof(Username));

            if (string.IsNullOrWhiteSpace(Password))
                yield return BusinessErrors.FieldNullOrEmpty(nameof(Password));
        }
    }
}