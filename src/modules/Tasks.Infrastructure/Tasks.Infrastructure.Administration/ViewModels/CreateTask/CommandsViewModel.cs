using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Autofac;
using Orcus.Administration.Library.Extensions;
using Orcus.Administration.Library.Services;
using Prism.Commands;
using Tasks.Infrastructure.Administration.Library;
using Tasks.Infrastructure.Administration.Library.Command;

namespace Tasks.Infrastructure.Administration.ViewModels.CreateTask
{
    public class CommandsViewModel : TaskServicesViewModel
    {
        private readonly IWindowService _windowService;
        private readonly IComponentContext _container;
        private readonly ObservableCollection<TaskViewModelView> _childs;
        private readonly IReadOnlyList<ICommandViewProvider> _viewProviders;
        private TaskViewModelView _selectedChild;
        private ITaskServiceDescription _selectedService;

        public CommandsViewModel(IWindowService windowService, IComponentContext container)
        {
            _windowService = windowService;
            _container = container;
            _childs = new ObservableCollection<TaskViewModelView>();

            Childs = new ListCollectionView(_childs);

            AvailableServices = container.Resolve<IEnumerable<ICommandDescription>>().Cast<ITaskServiceDescription>().ToList();
            _viewProviders = container.Resolve<IEnumerable<ICommandViewProvider>>().OrderByDescending(x => x.Priority).ToList();
        }

        public override ListCollectionView Childs { get; }
        public override IList<ITaskServiceDescription> AvailableServices { get; }

        public override TaskViewModelView SelectedChild
        {
            get => _selectedChild;
            set
            {
                var oldValue = _selectedChild;
                if (SetProperty(ref _selectedChild, value))
                {
                    if (oldValue != null)
                        oldValue.IsSelected = false;
                    if (value != null)
                        value.IsSelected = true;
                }
            }
        }

        public override ITaskServiceDescription SelectedService
        {
            get => _selectedService;
            set
            {
                if (SetProperty(ref _selectedService, value))
                {
                    if (value == null)
                        SelectedChild = null;
                    else
                    {
                        var viewModelView = CreateView(value);
                        if (_childs.Any())
                            RemoveChild(_childs.Single());
                        AddChild(viewModelView);

                        SelectedChild = viewModelView;
                    }
                }
            }
        }

        private DelegateCommand _addNewCommand;

        public override ICommand AddNewCommand
        {
            get
            {
                return _addNewCommand ?? (_addNewCommand = new DelegateCommand(() =>
                {
                    var viewModel = new TaskCreateServiceViewModel(this);
                    if (_windowService.ShowDialog(viewModel) == true)
                    {
                        AddChild(viewModel.View);
                    }
                }));
            }
        }

        public override TaskViewModelView CreateView(ITaskServiceDescription description)
        {
            var viewModelType = typeof(ICommandViewModel<>).MakeGenericType(description.DtoType);
            var viewModel = _container.Resolve(viewModelType);

            UIElement view = null;
            foreach (var viewProvider in _viewProviders)
            {
                view = viewProvider.GetView(viewModel, _container);
                if (view != null)
                    break;
            }

            return new TaskViewModelView(viewModel, view, description, this);
        }

        private bool _isSelected;

        public override bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (SetProperty(ref _isSelected, value))
                {
                    if (value && _childs.Count > 1)
                        SelectedChild = null;
                }
            }
        }

        private void AddChild(TaskViewModelView taskViewModelView)
        {
            _childs.Add(taskViewModelView);
            taskViewModelView.PropertyChanged += ChildOnPropertyChanged;
        }

        private void RemoveChild(TaskViewModelView taskViewModelView)
        {
            _childs.Remove(taskViewModelView);
            taskViewModelView.PropertyChanged -= ChildOnPropertyChanged;
        }

        private void ChildOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(TaskViewModelView.IsSelected):
                    var taskViewModel = (TaskViewModelView) sender;
                    if (taskViewModel.IsSelected)
                        SelectedChild = taskViewModel;
                    break;
            }
        }
    }
}