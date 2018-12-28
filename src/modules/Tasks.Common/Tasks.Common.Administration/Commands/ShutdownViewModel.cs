using System.ComponentModel.DataAnnotations;
using Tasks.Common.Shared.Commands;
using Tasks.Infrastructure.Administration.Library.Command;
using Tasks.Infrastructure.Core;

namespace Tasks.Common.Administration.Commands
{
    public class ShutdownViewModel : ICommandViewModel<ShutdownCommandInfo>
    {
        public void Initialize(ShutdownCommandInfo model)
        {
        }

        public ValidationResult ValidateInput() => ValidationResult.Success;
        public ValidationResult ValidateContext(MazeTask mazeTask) => ValidationResult.Success;

        public ShutdownCommandInfo Build() => new ShutdownCommandInfo();
    }
}