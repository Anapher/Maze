using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Orcus.Administration.Library.Extensions;
using Orcus.Administration.Library.Services;
using Orcus.Administration.Library.Views;
using Prism.Commands;
using Prism.Mvvm;
using Tasks.Infrastructure.Administration.ViewModels.CreateTask;
using Tasks.Infrastructure.Administration.ViewModels.CreateTask.Base;
using Tasks.Infrastructure.Core;

namespace Tasks.Infrastructure.Administration.ViewModels
{
    public class CreateTaskViewModel : BindableBase
    {
        private readonly IDialogWindow _window;
        private readonly IWindowService _windowService;
        private DelegateCommand _createTaskCommand;

        public CreateTaskViewModel(IWindowService windowService, IDialogWindow window, IComponentContext container)
        {
            _windowService = windowService;
            _window = window;
            TreeViewModels = new List<ITaskConfiguringViewModel>
            {
                new TaskSettingsViewModel(),
                new CommandsViewModel(windowService, window, container),
                new AudienceViewModel(),
                new TriggersViewModel(windowService, window, container),
                new FiltersViewModel(windowService, window, container),
                new StopEventsViewModel(windowService, window, container)
            };
        }

        public List<ITaskConfiguringViewModel> TreeViewModels { get; }

        public DelegateCommand CreateTaskCommand
        {
            get
            {
                return _createTaskCommand ?? (_createTaskCommand = new DelegateCommand(() =>
                {
                    var errors = TreeViewModels.SelectMany(x => x.ValidateInput()).ToList();
                    if (errors.Any())
                    {
                        _window.ShowErrorMessageBox(string.Join(Environment.NewLine, errors.Select(x => x.ErrorMessage)));
                        return;
                    }

                    var task = new OrcusTask();
                    foreach (var viewModel in TreeViewModels)
                        viewModel.Apply(task);

                    errors = TreeViewModels.SelectMany(x => x.ValidateContext(task)).ToList();
                    if (errors.Any())
                    {
                        _window.ShowErrorMessageBox(string.Join(Environment.NewLine, errors.Select(x => x.ErrorMessage)));
                        return;
                    }
                }));
            }
        }
    }
}