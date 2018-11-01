using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Prism.Commands;
using Prism.Mvvm;
using Tasks.Infrastructure.Administration.Library;
using Tasks.Infrastructure.Administration.Utilities;

namespace Tasks.Infrastructure.Administration.ViewModels.CreateTask
{
    public class TaskCreateServiceViewModel : BindableBase
    {
        private readonly TaskServicesViewModel _taskServices;
        private DelegateCommand _createCommand;
        private bool? _dialogResult;
        private ITaskServiceDescription _selectedService;
        private TaskViewModelView _view;

        public TaskCreateServiceViewModel(TaskServicesViewModel taskServices)
        {
            _taskServices = taskServices;
            AvailableServices = taskServices.AvailableServices;
        }

        public IList<ITaskServiceDescription> AvailableServices { get; }

        public ITaskServiceDescription SelectedService
        {
            get => _selectedService;
            set
            {
                if (SetProperty(ref _selectedService, value))
                {
                    if (value == null)
                    {
                        View = null;
                    }
                    else
                    {
                        var viewModelView = _taskServices.CreateView(value);
                        View = viewModelView;
                    }
                }
            }
        }

        public TaskViewModelView View
        {
            get => _view;
            set => SetProperty(ref _view, value);
        }

        public bool? DialogResult
        {
            get => _dialogResult;
            set => SetProperty(ref _dialogResult, value);
        }

        public DelegateCommand CreateCommand
        {
            get
            {
                return _createCommand ?? (_createCommand = new DelegateCommand(() =>
                {
                    var result = TaskServiceViewModelUtils.Validate(View.ViewModel, new TaskContext());
                    if (result == ValidationResult.Success) DialogResult = true;
                }));
            }
        }
    }
}