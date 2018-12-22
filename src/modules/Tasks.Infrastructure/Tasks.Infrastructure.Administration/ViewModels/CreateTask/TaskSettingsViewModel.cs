using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Prism.Mvvm;
using Tasks.Infrastructure.Administration.ViewModels.CreateTask.Base;
using Tasks.Infrastructure.Core;
using Unclassified.TxLib;

namespace Tasks.Infrastructure.Administration.ViewModels.CreateTask
{
    public class TaskSettingsViewModel : BindableBase, ITaskConfiguringViewModel
    {
        private bool _executeOnce;
        private string _name;
        private Guid? _taskId;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public bool ExecuteOnce
        {
            get => _executeOnce;
            set => SetProperty(ref _executeOnce, value);
        }

        public bool IsSelected { get; set; }
        public object NodeViewModel => this;

        public void Initialize(OrcusTask orcusTask)
        {
            Name = orcusTask.Name;
            ExecuteOnce = orcusTask.ExecuteOnce;
            _taskId = orcusTask.Id;
        }

        public IEnumerable<ValidationResult> ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(Name))
                yield return new ValidationResult(Tx.T("TasksInfrastructure:CreateTask.Validation.NameCannotBeEmpty"));
        }

        public IEnumerable<ValidationResult> ValidateContext(OrcusTask orcusTask)
        {
            return Enumerable.Empty<ValidationResult>();
        }

        public void Apply(OrcusTask orcusTask)
        {
            orcusTask.Name = Name;
            orcusTask.ExecuteOnce = ExecuteOnce;
            orcusTask.Id = _taskId ?? Guid.NewGuid();
        }
    }
}