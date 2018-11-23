using System.ComponentModel.DataAnnotations;
using Tasks.Common.Administration.Commands.Base;
using Tasks.Common.Administration.Commands.Utils;
using Tasks.Common.Shared.Commands;
using Tasks.Infrastructure.Administration.Library.Command;
using Tasks.Infrastructure.Core;

namespace Tasks.Common.Administration.Commands
{
    public class DownloadAndExecuteViewModel : ProcessViewModelBase<DownloadAndExecuteCommandInfo>, ICommandViewModel<DownloadAndExecuteCommandInfo>
    {
        public DownloadAndExecuteViewModel()
        {
            FileSource = new FileSourceViewModel();
            this.RegisterFileSource(FileSource);
        }

        public FileSourceViewModel FileSource { get; }

        public DownloadAndExecuteCommandInfo Build()
        {
            var result = new DownloadAndExecuteCommandInfo {FileSource = FileSource.Build()};
            ApplyProcessProperties(result);

            return result;
        }

        public ValidationResult ValidateContext(OrcusTask orcusTask) => ValidationResult.Success;

        public override void Initialize(DownloadAndExecuteCommandInfo model)
        {
            base.Initialize(model);

            FileSource.Initialize(model.FileSource);
        }

        public override ValidationResult ValidateInput()
        {
            var result = base.ValidateInput();
            if (result != ValidationResult.Success)
                return result;

            result = FileSource.ValidateInput();
            if (result != ValidationResult.Success)
                return result;

            return ValidationResult.Success;
        }
    }
}