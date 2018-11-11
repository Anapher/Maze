using System;
using Tasks.Infrastructure.Core.Dtos;

namespace Tasks.Infrastructure.Administration.ViewModels.Overview
{
    public class TaskViewModel
    {
        public TaskViewModel(TaskInfoDto taskInfo)
        {
            Name = taskInfo.Name;
            Id = taskInfo.Id;
            Commands = taskInfo.Commands;
            TotalExecutions = taskInfo.TotalExecutions;
            IsActive = taskInfo.IsActive;
        }

        public string Name { get; }
        public Guid Id { get; }
        public int Commands { get; set; }
        public int TotalExecutions { get; set; }
        public bool IsActive { get; set; }
    }
}