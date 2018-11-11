using System;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using Tasks.Infrastructure.Core.Dtos;

namespace Tasks.Infrastructure.Administration.ViewModels.TaskOverview
{
    public class TaskSessionViewModel : BindableBase
    {
        private ObservableCollection<TaskExecutionViewModel> _executions;

        public TaskSessionViewModel(TaskSessionDto taskSessionDto)
        {
            TaskSessionId = taskSessionDto.TaskSessionId;
            Description = taskSessionDto.Description;
            CreatedOn = taskSessionDto.CreatedOn;
        }

        public string TaskSessionId { get; }
        public string Description { get; }
        public DateTimeOffset CreatedOn { get; }

        public ObservableCollection<TaskExecutionViewModel> Executions
        {
            get => _executions;
            set => SetProperty(ref _executions, value);
        }
    }
}