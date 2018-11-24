using System;

namespace Tasks.Infrastructure.Administration.ViewModels.TaskOverview
{
    public interface ICommandStatusViewModel
    {
        Guid CommandResultId { get; }
        Guid TaskExecutionId { get; }
    }
}