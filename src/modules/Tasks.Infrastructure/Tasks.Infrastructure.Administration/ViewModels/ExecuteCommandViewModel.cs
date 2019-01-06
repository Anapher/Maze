using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using Anapher.Wpf.Toolkit.Windows;
using Maze.Administration.Library.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Prism.Commands;
using Prism.Mvvm;
using Tasks.Infrastructure.Administration.Library;
using Tasks.Infrastructure.Administration.Library.Command;
using Tasks.Infrastructure.Administration.Utilities;
using Tasks.Infrastructure.Administration.ViewModels.CreateTask;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Core.Audience;
using Tasks.Infrastructure.Core.Commands;
using Unclassified.TxLib;

namespace Tasks.Infrastructure.Administration.ViewModels
{
    public class ExecuteCommandViewModel : BindableBase
    {
        private readonly IServiceProvider _container;
        private readonly IWindowService _windowService;
        private AudienceCollection _audienceCollection;
        private bool? _dialogResult;
        private DelegateCommand _executeCommand;

        public ExecuteCommandViewModel(IWindowService windowService, IServiceProvider container)
        {
            _windowService = windowService;
            _container = container;
        }

        private TaskViewModelView _view;

        public TaskViewModelView View
        {
            get => _view;
            set => SetProperty(ref _view, value);
        }
        public string Title { get; private set; }

        public MazeTask MazeTask { get; private set; }

        public bool? DialogResult
        {
            get => _dialogResult;
            set => SetProperty(ref _dialogResult, value);
        }

        public DelegateCommand ExecuteCommand
        {
            get
            {
                return _executeCommand ?? (_executeCommand = new DelegateCommand(() =>
                {
                    var validationResult = TaskServiceViewModelUtils.ValidateInput(View.ViewModel);
                    if (validationResult != ValidationResult.Success)
                    {
                        _windowService.ShowErrorMessageBox(validationResult.ErrorMessage);
                        return;
                    }

                    MazeTask = new MazeTask
                    {
                        Name = "<CommandExecution>",
                        Id = Guid.NewGuid(),
                        Audience = _audienceCollection,
                        Commands = new List<CommandInfo> {TaskServiceViewModelUtils.Build<CommandInfo>(View.ViewModel)}
                    };

                    DialogResult = true;
                }));
            }
        }

        public void Initialize(ICommandDescription commandDescription, AudienceCollection audienceCollection)
        {
            View = CreateView(commandDescription);
            _audienceCollection = audienceCollection;

            Title = Tx.T("TasksInfrastructure:ExecuteCommand", "name", commandDescription.Name);
        }

        private TaskViewModelView CreateView(ITaskServiceDescription description)
        {
            var viewProviders = _container.GetRequiredService<IEnumerable<ICommandViewProvider>>().OrderByDescending(x => x.Priority);

            var viewModelType = typeof(ICommandViewModel<>).MakeGenericType(description.DtoType);
            var viewModel = _container.GetRequiredService(viewModelType);

            UIElement view = null;
            foreach (var viewProvider in viewProviders)
            {
                view = viewProvider.GetView(viewModel, _container);
                if (view != null)
                    break;
            }

            return new TaskViewModelView(viewModel, view, description, this);
        }
    }
}