using System;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using Tasks.Infrastructure.Core.Dtos;

namespace Tasks.Infrastructure.Administration.ViewModels.Overview
{
    public class TaskViewModel : BindableBase
    {
        private DateTimeOffset? _lastExecution;
        private DateTimeOffset? _nextExecution;
        private int _totalExecutions;
        private bool _isEnabled;
        private bool _isCompletedOnServer;

        public TaskViewModel(TaskInfoDto taskInfo)
        {
            Name = taskInfo.Name;
            Id = taskInfo.Id;
            Commands = taskInfo.Commands;
            TotalExecutions = taskInfo.TotalExecutions;
            IsEnabled = taskInfo.IsEnabled;
            IsCompletedOnServer = taskInfo.IsCompletedOnServer;
            Sessions = new ObservableCollection<string>(taskInfo.Sessions);
            LastExecution = taskInfo.LastExecution;
            AddedOn = taskInfo.AddedOn;
        }

        public string Name { get; }
        public Guid Id { get; }
        public int Commands { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset AddedOn { get; }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        public bool IsCompletedOnServer
        {
            get => _isCompletedOnServer;
            set => SetProperty(ref _isCompletedOnServer, value);
        }

        public int TotalExecutions
        {
            get => _totalExecutions;
            set => SetProperty(ref _totalExecutions, value);
        }

        public DateTimeOffset? NextExecution
        {
            get => _nextExecution;
            set => SetProperty(ref _nextExecution, value);
        }

        public DateTimeOffset? LastExecution
        {
            get => _lastExecution;
            set => SetProperty(ref _lastExecution, value);
        }

        public ObservableCollection<string> Sessions { get; }
    }
}