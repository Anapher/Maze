using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Maze.Server.Connection;

namespace Tasks.Infrastructure.Core.Dtos
{
    public class TaskExecutionDto : IValidatableObject
    {
        public Guid TaskExecutionId { get; set; }

        public string TaskSessionId { get; set; }
        public Guid TaskReferenceId { get; set; }

        public int? TargetId { get; set; }
        public DateTimeOffset CreatedOn { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TaskExecutionId == default)
                yield return new ValidationResult("The task execution id must not be zero.");

            if (!Hash.TryParse(TaskSessionId, out var hash) || hash.HashData.Length != 16)
                yield return new ValidationResult("The task session id must be a 128 bit hash value.");

            if(TaskReferenceId == Guid.Empty)
                yield return new ValidationResult("A task reference id is required.");
        }
    }
}