using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Orcus.Server.Connection;
using Tasks.Infrastructure.Core.Audience;
using Tasks.Infrastructure.Core.Commands;
using Tasks.Infrastructure.Core.Filter;
using Tasks.Infrastructure.Core.StopEvents;
using Tasks.Infrastructure.Core.Triggers;

namespace Tasks.Infrastructure.Core
{
    /// <summary>
    ///     An Orcus Task
    /// </summary>
    public class OrcusTask : IValidatableObject
    {
        /// <summary>
        ///     The name of the task. This value must not be unique and may only be used for displaying to the user
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     The unique <see cref="Guid"/> for the task
        /// </summary>
        public Guid Id { get; set; }
        public bool ExecuteOnce { get; set; }
        public TimeSpan? RestartOnFailInterval { get; set; }
        public int? MaximumRestarts { get; set; }

        /// <summary>
        ///     The audience of the task that decides who should execute the commands.
        /// </summary>
        public AudienceCollection Audience { get; set; }

        /// <summary>
        ///     Filters for the audience that can apply more specific conditions
        /// </summary>
        public IList<FilterInfo> Filters { get; set; }

        /// <summary>
        ///     Triggers decide when the commands should be executed
        /// </summary>
        public IList<TriggerInfo> Triggers { get; set; }

        /// <summary>
        ///     Stop events automatically cancel running commands
        /// </summary>
        public IList<StopEventInfo> StopEvents { get; set; }

        /// <summary>
        ///     The commands
        /// </summary>
        public IList<CommandInfo> Commands { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Name))
                yield return BusinessErrors.FieldNullOrEmpty("The name may not be empty");

            if (Id == Guid.Empty)
                yield return TaskErrors.TaskGuidEmpty;

            if (RestartOnFailInterval != null && RestartOnFailInterval <= TimeSpan.Zero)
                yield return TaskErrors.RestartOnFailIntervalMustBePositive;

            if (RestartOnFailInterval == null && MaximumRestarts < 1)
                yield return TaskErrors.MaximumRestartsMustBeGreaterThanZero;

            if (!Audience.IncludesServer && !Audience.IsAll && !Audience.Any())
                yield return TaskErrors.NoAudienceGiven;

            if (!Triggers.Any())
                yield return TaskErrors.NoTriggersGiven;

            if (!Commands.Any())
                yield return TaskErrors.NoCommandsGiven;
        }
    }
}