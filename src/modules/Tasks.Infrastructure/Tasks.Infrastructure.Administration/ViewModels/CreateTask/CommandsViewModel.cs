using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Autofac;
using Maze.Administration.Library.Views;
using Tasks.Infrastructure.Administration.Library;
using Tasks.Infrastructure.Administration.Library.Command;
using Tasks.Infrastructure.Administration.Utilities;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Core.Commands;
using Unclassified.TxLib;

namespace Tasks.Infrastructure.Administration.ViewModels.CreateTask
{
    public class CommandsViewModel : TaskServicesBaseViewModel<ICommandDescription>
    {
        private readonly IReadOnlyList<ICommandViewProvider> _viewProviders;

        public CommandsViewModel(IWindowService windowService, IComponentContext container) : base(windowService, container)
        {
            _viewProviders = container.Resolve<IEnumerable<ICommandViewProvider>>().OrderByDescending(x => x.Priority).ToList();
        }

        public override TaskViewModelView CreateView(ITaskServiceDescription description)
        {
            var viewModelType = typeof(ICommandViewModel<>).MakeGenericType(description.DtoType);
            var viewModel = Container.Resolve(viewModelType);

            UIElement view = null;
            foreach (var viewProvider in _viewProviders)
            {
                view = viewProvider.GetView(viewModel, Container);
                if (view != null)
                    break;
            }

            return new TaskViewModelView(viewModel, view, description, this);
        }

        public override string EntryName { get; } = Tx.T("TasksInfrastructure:CreateTask.Commands", 1);

        public override void Initialize(MazeTask mazeTask)
        {
            foreach (var commandInfo in mazeTask.Commands)
            {
                var commandInfoType = commandInfo.GetType();
                var description = AvailableServices.First(x => x.DtoType == commandInfoType);
                var view = CreateView(description);

                TaskServiceViewModelUtils.Initialize(view.ViewModel, commandInfo);
                AddChild(view);

                if (mazeTask.Commands.Count == 1)
                {
                    SetProperty(ref _selectedService, description, nameof(SelectedService));
                    SetProperty(ref _selectedChild, view, nameof(SelectedChild));
                }
            }
        }

        public override void Apply(MazeTask mazeTask)
        {
            mazeTask.Commands = new List<CommandInfo>();
            foreach (var taskView in _childs)
            {
                var dto = TaskServiceViewModelUtils.Build<CommandInfo>(taskView.ViewModel);
                mazeTask.Commands.Add(dto);
            }
        }
    }
}