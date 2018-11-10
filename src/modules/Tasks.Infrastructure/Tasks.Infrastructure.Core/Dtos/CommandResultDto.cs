using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Tasks.Infrastructure.Core.Dtos
{
    public class CommandResultDto : IValidatableObject
    {
        public Guid CommandResultId { get; set; }
        public Guid TaskExecutionId { get; set; }

        public string CommandName { get; set; }
        public string Result { get; set; }
        public int? Status { get; set; }
        public DateTimeOffset FinishedAt { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CommandResultId == default)
                yield return new ValidationResult("The command result id must not be zero.");

            if (TaskExecutionId == default)
                yield return new ValidationResult("The task execution id must not be zero.");

            if (string.IsNullOrWhiteSpace(CommandName))
                yield return new ValidationResult("The command name must not be empty.");

            if(Status != null)
                if (Result.Length % 4 != 0 || !Regex.IsMatch(Result, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None))
                    yield return new ValidationResult("The result is not a valid Base64 encoded string.");
        }
    }
}