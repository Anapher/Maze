using System;
using System.Windows;
using Prism.Mvvm;
using Tasks.Infrastructure.Core.Dtos;

namespace Tasks.Infrastructure.Administration.ViewModels.TaskOverview
{
    public class CommandResultViewModel : BindableBase, ICommandStatusViewModel
    {
        private UIElement _view;

        public CommandResultViewModel(CommandResultDto commandResult)
        {
            CommandResult = commandResult;
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

        public CommandResultDto CommandResult { get; }

        public string CommandName { get; }
        public string Result { get; }
        public int? StatusCode { get; }
        public DateTimeOffset FinishedAt { get; }
        public CommandStatus Status { get; }

        public UIElement View
        {
            get => _view;
            set => SetProperty(ref _view, value);
        }

        public Guid CommandResultId { get; }
        public Guid TaskExecutionId { get; }
    }
}