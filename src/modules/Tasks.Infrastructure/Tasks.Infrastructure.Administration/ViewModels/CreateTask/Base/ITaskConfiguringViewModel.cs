using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Tasks.Infrastructure.Core;

namespace Tasks.Infrastructure.Administration.ViewModels.CreateTask.Base
{
    public interface ITaskConfiguringViewModel : ITreeViewItem
    {
        void Initialize(MazeTask mazeTask);
        IEnumerable<ValidationResult> ValidateInput();
        IEnumerable<ValidationResult> ValidateContext(MazeTask mazeTask);
        void Apply(MazeTask mazeTask);
    }
}