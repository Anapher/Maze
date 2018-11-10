using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Orcus.Server.Connection;

namespace Tasks.Infrastructure.Core.Dtos
{
    public class TaskSessionDto : IValidatableObject
    {
        public string TaskSessionId { get; set; }
        public Guid TaskReferenceId { get; set; }

        public string Description { get; set; }
        public DateTimeOffset CreatedOn { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(TaskSessionId) || !Hash.TryParse(TaskSessionId, out var hash) || !hash.IsSha256Hash)
                yield return new ValidationResult("The task session id must be a SHA256 hash value.");

            if (TaskReferenceId == default)
                yield return new ValidationResult("The task reference id must not be zero.");

            if (string.IsNullOrWhiteSpace(Description))
                yield return new ValidationResult("The description must not be empty.");
        }
    }
}