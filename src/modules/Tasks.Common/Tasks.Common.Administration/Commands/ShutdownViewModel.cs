using System;
using System.ComponentModel.DataAnnotations;
using Tasks.Common.Shared.Commands;
using Tasks.Infrastructure.Administration.Library;
using Tasks.Infrastructure.Administration.Library.Command;

namespace Tasks.Common.Administration.Commands
{
    public class ShutdownViewModel : ICommandViewModel<ShutdownCommandInfo>
    {
        public void Initialize(ShutdownCommandInfo model)
        {
        }

        public ValidationResult Validate(TaskContext context) => ValidationResult.Success;

        public ShutdownCommandInfo Build(TaskContext context) => throw new NotImplementedException();
    }
}