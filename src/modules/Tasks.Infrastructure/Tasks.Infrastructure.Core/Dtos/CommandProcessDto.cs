using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tasks.Infrastructure.Core.Dtos
{
    public class CommandProcessDto : IValidatableObject
    {
        public Guid CommandResultId { get; set; }
        public Guid TaskExecutionId { get; set; }

        public string CommandName { get; set; }
        public double? Progress { get; set; }
        public string StatusMessage { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CommandResultId == default)
                yield return new ValidationResult("The command result id must not be zero.");

            if (string.IsNullOrWhiteSpace(CommandName))
                yield return new ValidationResult("The command name must not be empty.");

            if (TaskExecutionId == default)
                yield return new ValidationResult("The task execution id must not be zero.");
        }
    }
}