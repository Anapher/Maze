using System.ComponentModel.DataAnnotations;
using Tasks.Common.Triggers;
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
        public ValidationResult ValidateContext(MazeTask mazeTask) => ValidationResult.Success;
        public OnJoinTriggerInfo Build() => new OnJoinTriggerInfo();
    }
}