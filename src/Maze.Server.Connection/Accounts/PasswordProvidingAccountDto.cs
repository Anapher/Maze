using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Maze.Server.Connection.Accounts
{
    public class PasswordProvidingAccountDto : AccountDto
    {
        public string Password { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            foreach (var validationResult in base.Validate(validationContext))
                yield return validationResult;

            if (string.IsNullOrWhiteSpace(Password))
                yield return BusinessErrors.FieldNullOrEmpty(nameof(Password));
        }
    }
}