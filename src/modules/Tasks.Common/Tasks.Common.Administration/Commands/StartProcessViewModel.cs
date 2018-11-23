using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Orcus.Utilities;
using Tasks.Common.Administration.Commands.Base;
using Tasks.Common.Shared.Commands;
using Tasks.Infrastructure.Administration.Library.Command;
using Tasks.Infrastructure.Administration.PropertyGrid;
using Tasks.Infrastructure.Core;
using Unclassified.TxLib;

namespace Tasks.Common.Administration.Commands
{
    public class StartProcessViewModel : ProcessViewModelBase<StartProcessCommandInfo>, ICommandViewModel<StartProcessCommandInfo>
    {
        public StartProcessViewModel()
        {
            this.RegisterProperty(() => FileName, Tx.T("TasksCommon:Commands.StartProcess.Properties.FileName"),
                Tx.T("TasksCommon:Commands.StartProcess.Properties.FileName.Description"), Tx.T("TasksCommon:Commands.StartProcess.File"));
        }

        public string FileName { get; set; }

        public StartProcessCommandInfo Build()
        {
            var result = new StartProcessCommandInfo {FileName = FileName};
            ApplyProcessProperties(result);

            return result;
        }

        public override void Initialize(StartProcessCommandInfo model)
        {
            base.Initialize(model);

            FileName = model.FileName;
        }

        public ValidationResult ValidateContext(OrcusTask orcusTask) => ValidationResult.Success;

        public override ValidationResult ValidateInput()
        {
            var result = base.ValidateInput();
            if (result != ValidationResult.Success)
                return result;

            try
            {
                new FileInfo(FileName);
            }
            catch (Exception)
            {
                return new ValidationResult(Tx.T("TasksCommon:Commands.StartProcess.Errors.InvalidFileName"), nameof(FileName).Yield());
            }

            return ValidationResult.Success;
        }
    }
}