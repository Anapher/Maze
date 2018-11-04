using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Extensions;
using Orcus.Administration.Library.Services;
using Orcus.Administration.Library.Views;
using Orcus.Server.Connection.Utilities;
using Prism.Commands;
using Prism.Mvvm;
using Tasks.Infrastructure.Administration.Rest.V1;
using Tasks.Infrastructure.Administration.ViewModels.CreateTask;
using Tasks.Infrastructure.Administration.ViewModels.CreateTask.Base;
using Tasks.Infrastructure.Core;

namespace Tasks.Infrastructure.Administration.ViewModels
{
    public class CreateTaskViewModel : BindableBase
    {
        private readonly IDialogWindow _window;
        private readonly IComponentContext _container;
        private readonly IWindowService _windowService;
        private DelegateCommand _createTaskCommand;

        public CreateTaskViewModel(IWindowService windowService, IDialogWindow window, IComponentContext container)
        {
            _windowService = windowService;
            _window = window;
            _container = container;
            TreeViewModels = new List<ITaskConfiguringViewModel>
            {
                new TaskSettingsViewModel{IsSelected = true},
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
                return _createTaskCommand ?? (_createTaskCommand = new DelegateCommand(async () =>
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

                    var componentResolver = _container.Resolve<ITaskComponentResolver>();
                    var xmlCache = _container.Resolve<IXmlSerializerCache>();
                    var restClient = _container.Resolve<IRestClient>();

                    await TasksResource.Create(task, componentResolver, xmlCache, restClient);
                }));
            }
        }
    }
}