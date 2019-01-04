using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Maze.Server.Connection.Accounts
{
    public class AccountDto : IValidatableObject
    {
        public int AccountId { get; set; }
        public string Username { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime CreatedOn { get; set; }

        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(Username))
            {
                yield return BusinessErrors.FieldNullOrEmpty(nameof(Username));
            }
            else
            {
                if (Username.Length > LengthConsts.AccountUsernameMaxLength)
                    yield return BusinessErrors.Account.UsernameTooLong;

                if (!Regex.IsMatch(Username, "^[a-zA-Z0-9]+$"))
                    yield return BusinessErrors.Account.InvalidCharsInUsername;
            }
        }
    }
}