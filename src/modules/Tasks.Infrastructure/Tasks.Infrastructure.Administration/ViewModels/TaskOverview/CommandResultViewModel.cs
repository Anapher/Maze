using System;
using Prism.Mvvm;
using Tasks.Infrastructure.Core.Dtos;

namespace Tasks.Infrastructure.Administration.ViewModels.TaskOverview
{
    public class CommandResultViewModel : BindableBase, ICommandStatusViewModel
    {
        public CommandResultViewModel(CommandResultDto commandResult)
        {
            CommandResultId = commandResult.CommandResultId;
            TaskExecutionId = commandResult.TaskExecutionId;
            CommandName = commandResult.CommandName;
            Result = commandResult.Result;
            FinishedAt = commandResult.FinishedAt;
            StatusCode = commandResult.Status;

            if (StatusCode == null)
                Status = CommandStatus.Failure;
            else if (StatusCode >= 200 && StatusCode <= 299)
                Status = CommandStatus.Succeeded;
            else
                Status = CommandStatus.Error;
        }

        public Guid CommandResultId { get; }
        public Guid TaskExecutionId { get; }

        public string CommandName { get; }
        public string Result { get; }
        public int? StatusCode { get; }
        public DateTimeOffset FinishedAt { get; }
        public CommandStatus Status { get; }
    }

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

    public interface ICommandStatusViewModel
    {
        Guid CommandResultId { get; }
        Guid TaskExecutionId { get; }
    }

    public enum CommandStatus
    {
        Succeeded,
        Failure,
        Error,
        Warning
    }
}