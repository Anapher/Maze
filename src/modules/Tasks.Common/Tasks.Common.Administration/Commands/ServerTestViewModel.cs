using System.ComponentModel.DataAnnotations;
using Tasks.Common.Shared.Commands;
using Tasks.Infrastructure.Administration.Library.Command;
using Tasks.Infrastructure.Core;

namespace Tasks.Common.Administration.Commands
{
    public class ServerTestViewModel : ICommandViewModel<ServerTestCommandInfo>
    {
        public void Initialize(ServerTestCommandInfo model)
        {
        }

        public ValidationResult ValidateInput() => ValidationResult.Success;

        public ValidationResult ValidateContext(OrcusTask orcusTask) => ValidationResult.Success;

        public ServerTestCommandInfo Build() => new ServerTestCommandInfo();
    }
}