using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Input;
using Prism.Mvvm;
using Tasks.Infrastructure.Administration.Library;

namespace Tasks.Infrastructure.Administration.ViewModels.CreateTask
{
    public abstract class TaskServicesViewModel : BindableBase, ITreeViewItem
    {
        public abstract ListCollectionView Childs { get; }
        public abstract TaskViewModelView SelectedChild { get; set; }
        public abstract IList<ITaskServiceDescription> AvailableServices { get; }
        public abstract ITaskServiceDescription SelectedService { get; set; }
        public abstract ICommand AddNewCommand { get; }
        public abstract TaskViewModelView CreateView(ITaskServiceDescription description);

        public abstract bool IsSelected { get; set; }
        public object NodeViewModel => this;
    }
}