using System;
using System.ComponentModel.DataAnnotations;
using Tasks.Common.Shared.StopEvents;
using Tasks.Infrastructure.Administration.Library;
using Tasks.Infrastructure.Administration.Library.StopEvent;
using Tasks.Infrastructure.Administration.PropertyGrid;
using Tasks.Infrastructure.Core;
using Unclassified.TxLib;

namespace Tasks.Common.Administration.StopEvents
{
    public class DurationStopEventViewModel : PropertyGridViewModel, IStopEventViewModel<DurationStopEventInfo>
    {
        public DurationStopEventViewModel()
        {
            this.RegisterProperty(() => Duration, Tx.T("TasksCommon:StopEvents.Duration.Properties.Duration"),
                Tx.T("TasksCommon:StopEvents.Duration.Properties.Duration.Description"), Tx.T("TasksCommon:Categories.Common"));
        }

        public TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(10);

        public void Initialize(DurationStopEventInfo model)
        {
            Duration = model.Duration;

            OnPropertiesChanged();
        }

        public ValidationResult ValidateInput()
        {
            if (Duration <= TimeSpan.Zero)
                return new ValidationResult(Tx.T("TasksCommon:StopEvents.Duration.Errors.TimeSpanGreaterThanZero"));

            return ValidationResult.Success;
        }

        public ValidationResult ValidateContext(OrcusTask orcusTask) => ValidationResult.Success;

        public DurationStopEventInfo Build() => new DurationStopEventInfo {Duration = Duration};
    }
}