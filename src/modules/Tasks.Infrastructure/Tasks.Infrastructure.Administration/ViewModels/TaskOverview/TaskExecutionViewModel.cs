using System;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using Tasks.Infrastructure.Core.Dtos;

namespace Tasks.Infrastructure.Administration.ViewModels.TaskOverview
{
    public class TaskExecutionViewModel : BindableBase
    {
        private ObservableCollection<ICommandStatusViewModel> _results;

        public TaskExecutionViewModel(TaskExecutionDto execution)
        {
            TaskExecutionId = execution.TaskExecutionId;
            TaskSessionId = execution.TaskSessionId;
            TargetId = execution.TargetId;
            CreatedOn = execution.CreatedOn;
        }

        public Guid TaskExecutionId { get; }
        public string TaskSessionId { get; }
        public int? TargetId { get; }
        public DateTimeOffset CreatedOn { get; }

        public ObservableCollection<ICommandStatusViewModel> Results
        {
            get => _results;
            set => SetProperty(ref _results, value);
        }
    }
}