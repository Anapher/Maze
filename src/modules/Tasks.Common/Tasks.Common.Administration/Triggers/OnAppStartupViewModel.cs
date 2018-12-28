using System.ComponentModel.DataAnnotations;
using Tasks.Common.Triggers;
using Tasks.Infrastructure.Administration.Library.Trigger;
using Tasks.Infrastructure.Core;

namespace Tasks.Common.Administration.Triggers
{
    public class OnAppStartupViewModel : ITriggerViewModel<OnAppStartupTriggerInfo>
    {
        public void Initialize(OnAppStartupTriggerInfo model)
        {
        }

        public ValidationResult ValidateInput() => ValidationResult.Success;
        public ValidationResult ValidateContext(MazeTask mazeTask) => ValidationResult.Success;
        public OnAppStartupTriggerInfo Build() => new OnAppStartupTriggerInfo();
    }
}