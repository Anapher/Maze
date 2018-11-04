using System.ComponentModel.DataAnnotations;
using Tasks.Common.Shared.Triggers;
using Tasks.Infrastructure.Administration.Library.Trigger;
using Tasks.Infrastructure.Core;

namespace Tasks.Common.Administration.Triggers
{
    public class OnJoinViewModel : ITriggerViewModel<OnJoinTriggerInfo>
    {
        public void Initialize(OnJoinTriggerInfo model)
        {
        }

        public ValidationResult ValidateInput() => ValidationResult.Success;
        public ValidationResult ValidateContext(OrcusTask orcusTask) => ValidationResult.Success;
        public OnJoinTriggerInfo Build() => new OnJoinTriggerInfo();
    }
}