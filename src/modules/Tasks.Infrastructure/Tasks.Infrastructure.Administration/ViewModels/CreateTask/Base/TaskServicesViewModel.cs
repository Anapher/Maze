using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Windows.Data;
using System.Windows.Input;
using Prism.Mvvm;
using Tasks.Infrastructure.Administration.Library;
using Tasks.Infrastructure.Administration.ViewModels.CreateTask.Base;
using Tasks.Infrastructure.Core;

namespace Tasks.Infrastructure.Administration.ViewModels.CreateTask
{
    public abstract class TaskServicesViewModel : BindableBase, ITaskConfiguringViewModel
    {
        public abstract ListCollectionView Childs { get; }
        public abstract TaskViewModelView SelectedChild { get; set; }
        public abstract IList<ITaskServiceDescription> AvailableServices { get; }
        public abstract ITaskServiceDescription SelectedService { get; set; }
        public abstract ICommand AddNewCommand { get; }
        public abstract ICommand RemoveChildCommand { get; }
        public abstract TaskViewModelView CreateView(ITaskServiceDescription description);

        public abstract bool IsSelected { get; set; }
        public object NodeViewModel => this;
        public abstract string EntryName { get; }

        public abstract void Initialize(MazeTask mazeTask);
        public abstract IEnumerable<ValidationResult> ValidateInput();
        public abstract IEnumerable<ValidationResult> ValidateContext(MazeTask mazeTask);

        public abstract void Apply(MazeTask mazeTask);
    }
}