using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Orcus.Server.Connection.Tasks.Audience;
using Orcus.Server.Connection.Tasks.Commands;
using Orcus.Server.Connection.Tasks.Filter;
using Orcus.Server.Connection.Tasks.StopEvents;
using Orcus.Server.Connection.Tasks.Triggers;

namespace Orcus.Server.Connection.Tasks
{
    public class OrcusTask : IValidatableObject
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public bool ExecuteOnce { get; set; }
        public TimeSpan? RestartOnFailInterval { get; set; }
        public int? MaximumRestarts { get; set; }

        public AudienceCollection Audience { get; set; }
        public IList<FilterInfo> Filters { get; set; }
        public IList<TriggerInfo> Triggers { get; set; }
        public IList<StopEventInfo> StopEvents { get; set; }
        public IList<CommandInfo> Commands { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Name))
                yield return BusinessErrors.FieldNullOrEmpty("The name may not be empty");

            if (Id == Guid.Empty)
                yield return BusinessErrors.Tasks.TaskGuidEmpty;

            if (RestartOnFailInterval != null && RestartOnFailInterval <= TimeSpan.Zero)
                yield return BusinessErrors.Tasks.RestartOnFailIntervalMustBePositive;

            if (RestartOnFailInterval == null && MaximumRestarts < 1)
                yield return BusinessErrors.Tasks.MaximumRestartsMustBeGreaterThanZero;

            if (!Audience.IncludesServer && !Audience.IsAll && !Audience.Any())
                yield return BusinessErrors.Tasks.NoAudienceGiven;

            if (!Triggers.Any())
                yield return BusinessErrors.Tasks.NoTriggersGiven;

            if (!Commands.Any())
                yield return BusinessErrors.Tasks.NoCommandsGiven;
        }
    }
}