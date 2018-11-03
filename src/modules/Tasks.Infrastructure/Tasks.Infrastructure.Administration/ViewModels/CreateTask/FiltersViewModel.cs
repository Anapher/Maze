using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Autofac;
using Orcus.Administration.Library.Services;
using Orcus.Administration.Library.Views;
using Tasks.Infrastructure.Administration.Library;
using Tasks.Infrastructure.Administration.Library.Filter;
using Tasks.Infrastructure.Administration.Utilities;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Core.Filter;
using Unclassified.TxLib;

namespace Tasks.Infrastructure.Administration.ViewModels.CreateTask
{
    public class FiltersViewModel : TaskServicesBaseViewModel<IFilterDescription>
    {
        private readonly IReadOnlyList<IFilterViewProvider> _viewProviders;

        public FiltersViewModel(IWindowService windowService, IDialogWindow window, IComponentContext container) : base(windowService, window, container)
        {
            _viewProviders = container.Resolve<IEnumerable<IFilterViewProvider>>().OrderByDescending(x => x.Priority).ToList();
        }

        public override TaskViewModelView CreateView(ITaskServiceDescription description)
        {
            var viewModelType = typeof(IFilterViewModel<>).MakeGenericType(description.DtoType);
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

        public override string EntryName { get; } = Tx.T("TasksInfrastructure:CreateTask.Filters", 1);

        public override void Initialize(OrcusTask orcusTask)
        {
            foreach (var filterInfo in orcusTask.Filters)
            {
                var filterInfoType = filterInfo.GetType();
                var description = AvailableServices.First(x => x.DtoType == filterInfoType);
                var view = CreateView(description);

                TaskServiceViewModelUtils.Initialize(view.ViewModel, filterInfo);
                AddChild(view);
            }
        }

        public override void Apply(OrcusTask orcusTask)
        {
            foreach (var taskView in _childs)
            {
                var dto = TaskServiceViewModelUtils.Build<FilterInfo>(taskView.ViewModel, new TaskContext());
                orcusTask.Filters.Add(dto);
            }
        }
    }
}