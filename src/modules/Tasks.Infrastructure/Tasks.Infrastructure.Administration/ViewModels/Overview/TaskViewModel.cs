using System;
using Anapher.Wpf.Toolkit.Utilities;
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
        private int _commands;

        public TaskViewModel(TaskInfoDto taskInfo)
        {
            Name = taskInfo.Name;
            Id = taskInfo.Id;
            AddedOn = taskInfo.AddedOn;
            Sessions = new TransactionalObservableCollection<string>();

            Update(taskInfo);
        }

        public string Name { get; }
        public Guid Id { get; }
        public DateTimeOffset AddedOn { get; }

        public int Commands
        {
            get => _commands;
            set => SetProperty(ref _commands, value);
        }

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

        public TransactionalObservableCollection<string> Sessions { get; }

        public void Update(TaskInfoDto taskInfo)
        {
            Commands = taskInfo.Commands;
            TotalExecutions = taskInfo.TotalExecutions;
            IsEnabled = taskInfo.IsEnabled;
            IsCompletedOnServer = taskInfo.IsCompletedOnServer;

            Sessions.Clear();
            if (taskInfo.Sessions != null)
                Sessions.AddRange(taskInfo.Sessions);

            LastExecution = taskInfo.LastExecution;
        }
    }
}