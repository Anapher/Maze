using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Orcus.Utilities;
using Tasks.Common.Shared.Commands.Base;
using Tasks.Infrastructure.Administration.Library;
using Tasks.Infrastructure.Administration.PropertyGrid;
using Unclassified.TxLib;

namespace Tasks.Common.Administration.Commands.Base
{
    public abstract class ProcessViewModelBase<TCommandInfo> : PropertyGridViewModel where TCommandInfo : ProcessCommandInfoBase
    {
        protected ProcessViewModelBase()
        {
            this.RegisterProperty(() => WorkingDirectory, Tx.T("TasksCommon:Commands.ProcessBase.Properties.WorkingDirectory"),
                Tx.T("TasksCommon:Commands.ProcessBase.Properties.WorkingDirectory.Description"), Tx.T("TasksCommon:Commands.ProcessBase"));
            this.RegisterProperty(() => Arguments, Tx.T("TasksCommon:Commands.ProcessBase.Properties.Arguments"),
                Tx.T("TasksCommon:Commands.ProcessBase.Properties.Arguments.Description"), Tx.T("TasksCommon:Commands.ProcessBase"));
            this.RegisterProperty(() => Verb, Tx.T("TasksCommon:Commands.ProcessBase.Properties.Verb"),
                Tx.T("TasksCommon:Commands.ProcessBase.Properties.Verb.Description"), Tx.T("TasksCommon:Commands.ProcessBase"));
            this.RegisterProperty(() => UseShellExecute, Tx.T("TasksCommon:Commands.ProcessBase.Properties.UseShellExecute"),
                Tx.T("TasksCommon:Commands.ProcessBase.Properties.UseShellExecute.Description"), Tx.T("TasksCommon:Commands.ProcessBase"));
            this.RegisterProperty(() => CreateNoWindow, Tx.T("TasksCommon:Commands.ProcessBase.Properties.CreateNoWindow"),
                Tx.T("TasksCommon:Commands.ProcessBase.Properties.CreateNoWindow.Description"), Tx.T("TasksCommon:Commands.ProcessBase"));
            this.RegisterProperty(() => WaitForExit, Tx.T("TasksCommon:Commands.ProcessBase.Properties.WaitForExit"),
                Tx.T("TasksCommon:Commands.ProcessBase.Properties.WaitForExit.Description"), Tx.T("TasksCommon:Commands.ProcessBase"));
        }

        public string WorkingDirectory { get; set; }
        public string Arguments { get; set; }
        public string Verb { get; set; }
        public bool UseShellExecute { get; set; }
        public bool CreateNoWindow { get; set; }

        public bool WaitForExit { get; set; }

        public virtual void Initialize(TCommandInfo model)
        {
            WorkingDirectory = model.WorkingDirectory;
            Arguments = model.Arguments;
            Verb = model.Verb;
            UseShellExecute = model.UseShellExecute;
            CreateNoWindow = model.CreateNoWindow;
            WaitForExit = model.WaitForExit;

            OnPropertiesChanged();
        }

        protected void ApplyProcessProperties(TCommandInfo commandInfo)
        {
            commandInfo.WorkingDirectory = WorkingDirectory;
            commandInfo.Arguments = Arguments;
            commandInfo.Verb = Verb;
            commandInfo.UseShellExecute = UseShellExecute;
            commandInfo.CreateNoWindow = CreateNoWindow;
            commandInfo.WaitForExit = WaitForExit;
        }

        public virtual ValidationResult ValidateInput()
        {
            if (!string.IsNullOrWhiteSpace(WorkingDirectory))
                try
                {
                    new DirectoryInfo(WorkingDirectory);
                }
                catch (Exception)
                {
                    return new ValidationResult(Tx.T("TasksCommon:Commands.ProcessBase.Errors.InvalidWorkingDirectoryPath"),
                        nameof(WorkingDirectory).Yield());
                }

            return ValidationResult.Success;
        }
    }
}