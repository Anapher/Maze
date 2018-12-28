using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Autofac;
using Maze.Administration.Library.Views;
using Tasks.Infrastructure.Administration.Library;
using Tasks.Infrastructure.Administration.Library.Trigger;
using Tasks.Infrastructure.Administration.Utilities;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Core.Triggers;
using Unclassified.TxLib;

namespace Tasks.Infrastructure.Administration.ViewModels.CreateTask
{
    public class TriggersViewModel : TaskServicesBaseViewModel<ITriggerDescription>
    {
        private readonly IReadOnlyList<ITriggerViewProvider> _viewProviders;

        public TriggersViewModel(IWindowService windowService, IComponentContext container) : base(windowService, container)
        {
            _viewProviders = container.Resolve<IEnumerable<ITriggerViewProvider>>().OrderByDescending(x => x.Priority).ToList();
        }

        public override TaskViewModelView CreateView(ITaskServiceDescription description)
        {
            var viewModelType = typeof(ITriggerViewModel<>).MakeGenericType(description.DtoType);
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

        public override string EntryName { get; } = Tx.T("TasksInfrastructure:CreateTask.Triggers", 1);

        public override void Initialize(MazeTask mazeTask)
        {
            foreach (var triggerInfo in mazeTask.Triggers)
            {
                var triggerInfoType = triggerInfo.GetType();
                var description = AvailableServices.First(x => x.DtoType == triggerInfoType);
                var view = CreateView(description);

                TaskServiceViewModelUtils.Initialize(view.ViewModel, triggerInfo);
                AddChild(view);

                if (mazeTask.Triggers.Count == 1)
                {
                    SetProperty(ref _selectedService, description, nameof(SelectedService));
                    SetProperty(ref _selectedChild, view, nameof(SelectedChild));
                }
            }
        }

        public override void Apply(MazeTask mazeTask)
        {
            mazeTask.Triggers = new List<TriggerInfo>();
            foreach (var taskView in _childs)
            {
                var dto = TaskServiceViewModelUtils.Build<TriggerInfo>(taskView.ViewModel);
                mazeTask.Triggers.Add(dto);
            }
        }
    }
}