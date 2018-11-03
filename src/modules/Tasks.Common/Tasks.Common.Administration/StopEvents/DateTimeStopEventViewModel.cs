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
    public class DateTimeStopEventViewModel : PropertyGridViewModel, IStopEventViewModel<DateTimeStopEventInfo>
    {
        public DateTimeStopEventViewModel()
        {
            this.RegisterProperty(() => DateTime, Tx.T("TasksCommon:StopEvents.DateTime.Properties.DateTime"),
                Tx.T("TasksCommon:StopEvents.DateTime.Properties.DateTime.Description"), Tx.T("TasksCommon:Categories.Common"));

            DateTime = DateTime.Now.AddDays(1);
        }

        public DateTime DateTime { get; set; }

        public void Initialize(DateTimeStopEventInfo model)
        {
            DateTime = model.DateTime.LocalDateTime;
        }

        public ValidationResult ValidateInput()
        {
            if (DateTime < DateTime.Now)
                return new ValidationResult(Tx.T("TasksCommon:StopEvents.DateTime.Errors.DateTimeIsInPast"));

            return ValidationResult.Success;
        }

        public ValidationResult ValidateContext(OrcusTask orcusTask) => ValidationResult.Success;

        public DateTimeStopEventInfo Build() => new DateTimeStopEventInfo {DateTime = DateTime};
    }
}