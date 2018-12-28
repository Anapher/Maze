using System.ComponentModel.DataAnnotations;
using Tasks.Common.Shared.Commands;
using Tasks.Infrastructure.Administration.Library;
using Tasks.Infrastructure.Administration.Library.Command;
using Tasks.Infrastructure.Administration.PropertyGrid;
using Tasks.Infrastructure.Core;
using Unclassified.TxLib;

namespace Tasks.Common.Administration.Commands
{
    public class WakeOnLanViewModel : PropertyGridViewModel, ICommandViewModel<WakeOnLanCommandInfo>
    {
        public WakeOnLanViewModel()
        {
            this.RegisterProperty(() => TryOverClient, Tx.T("TasksCommon:Commands.WakeOnLan.Properties.TryOverClients"),
                Tx.T("TasksCommon:Commands.WakeOnLan.Properties.TryOverClients.Summary"), Tx.T("TasksCommon:Categories.Common"));
        }

        public bool TryOverClient { get; set; }

        public void Initialize(WakeOnLanCommandInfo model)
        {
            TryOverClient = model.TryOverClient;

            OnPropertiesChanged();
        }

        public ValidationResult ValidateInput() => ValidationResult.Success;
        public ValidationResult ValidateContext(MazeTask mazeTask) => ValidationResult.Success;
        public WakeOnLanCommandInfo Build() => new WakeOnLanCommandInfo{TryOverClient = TryOverClient};
    }
}