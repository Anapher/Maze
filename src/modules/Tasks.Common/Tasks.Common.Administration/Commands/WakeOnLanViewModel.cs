using System.ComponentModel.DataAnnotations;
using Tasks.Common.Shared.Commands;
using Tasks.Infrastructure.Administration.Library;
using Tasks.Infrastructure.Administration.Library.Command;
using Tasks.Infrastructure.Administration.PropertyGrid;
using Tasks.Infrastructure.Core;

namespace Tasks.Common.Administration.Commands
{
    public class WakeOnLanViewModel : PropertyGridViewModel, ICommandViewModel<WakeOnLanCommandInfo>
    {
        public WakeOnLanViewModel()
        {
            this.RegisterProperty(() => TryOverClient, "Try over Clients", "", "");
        }

        public void Initialize(WakeOnLanCommandInfo model)
        {
        }

        public bool TryOverClient { get; set; }

        public ValidationResult ValidateInput() => ValidationResult.Success;
        public ValidationResult ValidateContext(OrcusTask orcusTask) => ValidationResult.Success;
        public WakeOnLanCommandInfo Build() => new WakeOnLanCommandInfo();
    }
}