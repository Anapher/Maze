using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Orcus.Server.Connection;

namespace Tasks.Infrastructure.Core.Dtos
{
    public class CommandProcessDto : IValidatableObject
    {
        public Guid CommandResultId { get; set; }
        public string TaskSessionId { get; set; }
        public Guid TaskId { get; set; }
        public Guid TaskExecutionId { get; set; }

        public int? TargetId { get; set; }
        public double? Progress { get; set; }
        public string StatusMessage { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CommandResultId == default)
                yield return new ValidationResult("The command result id must not be zero.");

            if (string.IsNullOrWhiteSpace(TaskSessionId) || !Hash.TryParse(TaskSessionId, out var hash) || !hash.IsSha256Hash)
                yield return new ValidationResult("The task session id must be a SHA256 hash value.");

            if (TaskId == default)
                yield return new ValidationResult("The task id must not be zero.");

            if (TaskExecutionId == default)
                yield return new ValidationResult("The task execution id must not be zero.");
        }
    }
}
