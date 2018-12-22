using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Autofac;
using Orcus.Administration.Library.Views;
using Tasks.Infrastructure.Administration.Library;
using Tasks.Infrastructure.Administration.Library.StopEvent;
using Tasks.Infrastructure.Administration.Utilities;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Core.StopEvents;
using Unclassified.TxLib;

namespace Tasks.Infrastructure.Administration.ViewModels.CreateTask
{
    public class StopEventsViewModel : TaskServicesBaseViewModel<IStopEventDescription>
    {
        private readonly IReadOnlyList<IStopEventViewProvider> _viewProviders;

        public StopEventsViewModel(IWindowService windowService, IComponentContext container) : base(windowService, container)
        {
            _viewProviders = container.Resolve<IEnumerable<IStopEventViewProvider>>().OrderByDescending(x => x.Priority).ToList();
        }

        public override TaskViewModelView CreateView(ITaskServiceDescription description)
        {
            var viewModelType = typeof(IStopEventViewModel<>).MakeGenericType(description.DtoType);
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

        public override string EntryName { get; } = Tx.T("TasksInfrastructure:CreateTask.StopEvents", 1);

        public override void Initialize(OrcusTask orcusTask)
        {
            foreach (var stopEventInfo in orcusTask.StopEvents)
            {
                var stopEventInfoType = stopEventInfo.GetType();
                var description = AvailableServices.First(x => x.DtoType == stopEventInfoType);
                var view = CreateView(description);

                TaskServiceViewModelUtils.Initialize(view.ViewModel, stopEventInfo);
                AddChild(view);

                if (orcusTask.StopEvents.Count == 1)
                {
                    SetProperty(ref _selectedService, description, nameof(SelectedService));
                    SetProperty(ref _selectedChild, view, nameof(SelectedChild));
                }
            }
        }

        public override void Apply(OrcusTask orcusTask)
        {
            orcusTask.StopEvents = new List<StopEventInfo>();
            foreach (var taskView in _childs)
            {
                var dto = TaskServiceViewModelUtils.Build<StopEventInfo>(taskView.ViewModel);
                orcusTask.StopEvents.Add(dto);
            }
        }
    }
}