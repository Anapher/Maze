using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Autofac;
using Orcus.Administration.Library.Services;
using Orcus.Administration.Library.Views;
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

        public CommandsViewModel(IWindowService windowService, IDialogWindow window, IComponentContext container) : base(windowService, window,
            container)
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

        public override void Initialize(OrcusTask orcusTask)
        {
            foreach (var commandInfo in orcusTask.Commands)
            {
                var commandInfoType = commandInfo.GetType();
                var description = AvailableServices.First(x => x.DtoType == commandInfoType);
                var view = CreateView(description);

                TaskServiceViewModelUtils.Initialize(view.ViewModel, commandInfo);
                AddChild(view);
            }
        }

        public override void Apply(OrcusTask orcusTask)
        {
            foreach (var taskView in _childs)
            {
                var dto = TaskServiceViewModelUtils.Build<CommandInfo>(taskView.ViewModel, new TaskContext());
                orcusTask.Commands.Add(dto);
            }
        }
    }
}