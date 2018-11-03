using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Tasks.Infrastructure.Core;

namespace Tasks.Infrastructure.Administration.ViewModels.CreateTask.Base
{
    public interface ITaskConfiguringViewModel : ITreeViewItem
    {
        void Initialize(OrcusTask orcusTask);
        IEnumerable<ValidationResult> ValidateInput();
        IEnumerable<ValidationResult> ValidateContext(OrcusTask orcusTask);
        void Apply(OrcusTask orcusTask);
    }
}