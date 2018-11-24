using System;
using Prism.Mvvm;
using Tasks.Infrastructure.Core.Dtos;

namespace Tasks.Infrastructure.Administration.ViewModels.TaskOverview
{
    public class CommandProcessViewModel : BindableBase, ICommandStatusViewModel
    {
        private double? _progress;
        private string _statusMessage;

        public CommandProcessViewModel(CommandProcessDto commandProcess)
        {
            Progress = commandProcess.Progress;
            StatusMessage = commandProcess.StatusMessage;
            CommandResultId = commandProcess.CommandResultId;
            TaskExecutionId = commandProcess.TaskExecutionId;
            CommandName = commandProcess.CommandName;
        }

        public Guid CommandResultId { get; }
        public Guid TaskExecutionId { get; }
        public string CommandName { get; }

        public double? Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public void Update(CommandProcessDto commandProcess)
        {
            Progress = commandProcess.Progress;
            StatusMessage = commandProcess.StatusMessage;
        }
    }
}