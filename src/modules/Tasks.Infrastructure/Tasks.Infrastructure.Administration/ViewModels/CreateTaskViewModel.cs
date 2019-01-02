using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Maze.Administration.Library.Clients;
using Maze.Administration.Library.Extensions;
using Maze.Administration.Library.Services;
using Maze.Administration.Library.Views;
using Maze.Server.Connection.Utilities;
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
        private readonly IWindowService _windowService;
        private readonly IComponentContext _container;
        private DelegateCommand _createTaskCommand;
        private bool? _dialogResult;
        private bool _update;

        public CreateTaskViewModel(IWindowService windowService, IComponentContext container)
        {
            _windowService = windowService;
            _container = container;
            TreeViewModels = new List<ITaskConfiguringViewModel>
            {
                new TaskSettingsViewModel {IsSelected = true},
                new CommandsViewModel(windowService, container),
                new AudienceViewModel(container.Resolve<IClientManager>()),
                new TriggersViewModel(windowService, container),
                new FiltersViewModel(windowService, container),
                new StopEventsViewModel(windowService, container)
            };
        }

        public List<ITaskConfiguringViewModel> TreeViewModels { get; }

        public bool? DialogResult
        {
            get => _dialogResult;
            set => SetProperty(ref _dialogResult, value);
        }

        public bool Update
        {
            get => _update;
            set => SetProperty(ref _update, value);
        }

        public DelegateCommand CreateTaskCommand
        {
            get
            {
                return _createTaskCommand ?? (_createTaskCommand = new DelegateCommand(async () =>
                {
                    var errors = TreeViewModels.SelectMany(x => x.ValidateInput()).ToList();
                    if (errors.Any())
                    {
                        _windowService.ShowErrorMessageBox(string.Join(Environment.NewLine, errors.Select(x => x.ErrorMessage)));
                        return;
                    }

                    var task = new MazeTask();
                    foreach (var viewModel in TreeViewModels)
                        viewModel.Apply(task);

                    errors = TreeViewModels.SelectMany(x => x.ValidateContext(task)).ToList();
                    if (errors.Any())
                    {
                        _windowService.ShowErrorMessageBox(string.Join(Environment.NewLine, errors.Select(x => x.ErrorMessage)));
                        return;
                    }

                    var componentResolver = _container.Resolve<ITaskComponentResolver>();
                    var xmlCache = _container.Resolve<IXmlSerializerCache>();
                    var restClient = _container.Resolve<IRestClient>();

                    try
                    {
                        if (_update)
                        {
                            await TasksResource.Update(task, componentResolver, xmlCache, restClient);
                        }
                        else
                        {
                            await TasksResource.Create(task, componentResolver, xmlCache, restClient);
                        }

                        DialogResult = true;
                    }
                    catch (Exception e)
                    {
                        e.ShowMessage(_windowService);
                    }
                }));
            }
        }

        public void UpdateTask(MazeTask mazeTask)
        {
            foreach (var taskConfiguringViewModel in TreeViewModels)
            {
                taskConfiguringViewModel.Initialize(mazeTask);
            }

            Update = true;
        }
    }
}