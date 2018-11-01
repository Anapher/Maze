using Prism.Mvvm;
using Tasks.Infrastructure.Administration.Library;

namespace Tasks.Infrastructure.Administration.ViewModels.CreateTask
{
    public class TaskViewModelView : BindableBase, ITreeViewItem
    {
        private bool _isSelected;

        public TaskViewModelView(object viewModel, object view, ITaskServiceDescription description, object nodeViewModel)
        {
            ViewModel = viewModel;
            View = view;
            Description = description;
            NodeViewModel = nodeViewModel;
        }

        public object ViewModel { get; }
        public object View { get; }
        public ITaskServiceDescription Description { get; }
        public object NodeViewModel { get; }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}