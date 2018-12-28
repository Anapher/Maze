using System.ComponentModel.DataAnnotations;
using Tasks.Common.Triggers;
using Tasks.Infrastructure.Administration.Library.Trigger;
using Tasks.Infrastructure.Core;

namespace Tasks.Common.Administration.Triggers
{
    public class ImmediatelyViewModel : ITriggerViewModel<ImmediatelyTriggerInfo>
    {
        public void Initialize(ImmediatelyTriggerInfo model)
        {
        }

        public ValidationResult ValidateInput() => ValidationResult.Success;
        public ValidationResult ValidateContext(MazeTask mazeTask) => ValidationResult.Success;
        public ImmediatelyTriggerInfo Build() => new ImmediatelyTriggerInfo();
    }
}