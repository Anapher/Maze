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
        private TaskServicesViewModel _taskServices;
        private DelegateCommand _createCommand;
        private bool? _dialogResult;
        private ITaskServiceDescription _selectedService;
        private TaskViewModelView _view;

        public IList<ITaskServiceDescription> AvailableServices { get; private set; }

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
                        View = _taskServices.CreateView(value);
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
                    var result = TaskServiceViewModelUtils.ValidateInput(View.ViewModel);
                    if (result == ValidationResult.Success) DialogResult = true;
                }));
            }
        }

        public void Initialize(TaskServicesViewModel taskServices)
        {
            _taskServices = taskServices;
            AvailableServices = taskServices.AvailableServices;
        }
    }
}