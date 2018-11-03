using System.ComponentModel.DataAnnotations;
using Tasks.Common.Shared.Commands;
using Tasks.Infrastructure.Administration.Library.Command;
using Tasks.Infrastructure.Core;

namespace Tasks.Common.Administration.Commands
{
    public class WakeOnLanViewModel : ICommandViewModel<WakeOnLanCommandInfo>
    {
        public void Initialize(WakeOnLanCommandInfo model)
        {
        }

        public ValidationResult ValidateInput() => ValidationResult.Success;

        public ValidationResult ValidateContext(OrcusTask orcusTask) => ValidationResult.Success;

        public WakeOnLanCommandInfo Build() => new WakeOnLanCommandInfo();
    }
}