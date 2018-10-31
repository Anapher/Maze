using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Autofac;
using Prism.Commands;
using Prism.Mvvm;
using Tasks.Infrastructure.Administration.Library.Command;

namespace Tasks.Infrastructure.Administration.ViewModels.CreateTask
{
    public class CreateCommandViewModel : BindableBase
    {
        private readonly IComponentContext _container;
        private readonly IEnumerable<ICommandViewProvider> _viewProviders;

        public CreateCommandViewModel(IComponentContext container)
        {
            _container = container;
            Children = new ObservableCollection<CommandViewModel>();

            Commands = container.Resolve<IEnumerable<ICommandDescription>>().ToList();
            _viewProviders = container.Resolve<IEnumerable<ICommandViewProvider>>();

        }

        public ObservableCollection<CommandViewModel> Children { get; }
        public List<ICommandDescription> Commands { get; }

        private CommandViewModel _currentCommandViewModel;

        public CommandViewModel CurrentCommandViewModel
        {
            get => _currentCommandViewModel;
            set => SetProperty(ref _currentCommandViewModel, value);
        }

        private ICommandDescription _selectedCommand;

        public ICommandDescription SelectedCommand
        {
            get => _selectedCommand;
            set
            {
                if (SetProperty(ref _selectedCommand, value))
                {
                    if (value == null)
                        CurrentCommandViewModel = null;
                    else
                        CurrentCommandViewModel = CreateCommandVm(value);
                }
            }
        }

        private DelegateCommand _addAnotherCommand;

        public DelegateCommand AddAnotherCommand
        {
            get
            {
                return _addAnotherCommand ?? (_addAnotherCommand = new DelegateCommand(() =>
                {
                    Children.Add(CurrentCommandViewModel);
                    SelectedCommand = null;
                }));
            }
        }

        private CommandViewModel CreateCommandVm(ICommandDescription commandDescription)
        {
            var commandViewModel = new CommandViewModel {Description = commandDescription};

            var viewModelType = typeof(ICommandViewModel<>).MakeGenericType(commandDescription.DtoType);
            var viewModel = _container.Resolve(viewModelType);
            commandViewModel.ViewModel = viewModel;

            UIElement view = null;
            foreach (var viewProvider in _viewProviders)
            {
                view = viewProvider.GetView(viewModel, _container);
                if (view != null)
                    break;
            }

            commandViewModel.View = view;
            return commandViewModel;
        }
    }
    
    public class CommandViewModel
    {
        public object View { get; set; }
        public ICommandDescription Description { get; set; }
        public object ViewModel { get; set; }
    }
}