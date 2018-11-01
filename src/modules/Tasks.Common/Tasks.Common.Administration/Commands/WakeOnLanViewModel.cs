using System.ComponentModel.DataAnnotations;
using Tasks.Common.Shared.Commands;
using Tasks.Infrastructure.Administration.Library;
using Tasks.Infrastructure.Administration.Library.Command;

namespace Tasks.Common.Administration.Commands
{
    public class WakeOnLanViewModel : ICommandViewModel<WakeOnLanCommandInfo>
    {
        public void Initialize(WakeOnLanCommandInfo model)
        {
        }

        public ValidationResult Validate(TaskContext context) => ValidationResult.Success;
        public WakeOnLanCommandInfo Build(TaskContext context) => new WakeOnLanCommandInfo();
    }
}