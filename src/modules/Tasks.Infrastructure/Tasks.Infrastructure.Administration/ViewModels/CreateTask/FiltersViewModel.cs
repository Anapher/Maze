using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Anapher.Wpf.Toolkit.Windows;
using Microsoft.Extensions.DependencyInjection;
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

        public FiltersViewModel(IWindowService windowService, IServiceProvider serviceProvider) : base(windowService, serviceProvider)
        {
            _viewProviders = serviceProvider.GetRequiredService<IEnumerable<IFilterViewProvider>>().OrderByDescending(x => x.Priority).ToList();
        }

        public override TaskViewModelView CreateView(ITaskServiceDescription description)
        {
            var viewModelType = typeof(IFilterViewModel<>).MakeGenericType(description.DtoType);
            var viewModel = ServiceProvider.GetRequiredService(viewModelType);

            UIElement view = null;
            foreach (var viewProvider in _viewProviders)
            {
                view = viewProvider.GetView(viewModel, ServiceProvider);
                if (view != null)
                    break;
            }

            return new TaskViewModelView(viewModel, view, description, this);
        }

        public override string EntryName { get; } = Tx.T("TasksInfrastructure:CreateTask.Filters", 1);

        public override void Initialize(MazeTask mazeTask)
        {
            foreach (var filterInfo in mazeTask.Filters)
            {
                var filterInfoType = filterInfo.GetType();
                var description = AvailableServices.First(x => x.DtoType == filterInfoType);
                var view = CreateView(description);

                TaskServiceViewModelUtils.Initialize(view.ViewModel, filterInfo);
                AddChild(view);

                if (mazeTask.Filters.Count == 1)
                {
                    SetProperty(ref _selectedService, description, nameof(SelectedService));
                    SetProperty(ref _selectedChild, view, nameof(SelectedChild));
                }
            }
        }

        public override void Apply(MazeTask mazeTask)
        {
            mazeTask.Filters = new List<FilterInfo>();
            foreach (var taskView in _childs)
            {
                var dto = TaskServiceViewModelUtils.Build<FilterInfo>(taskView.ViewModel);
                mazeTask.Filters.Add(dto);
            }
        }
    }
}