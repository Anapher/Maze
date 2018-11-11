using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using Autofac;
using MahApps.Metro.IconPacks;
using Microsoft.Extensions.DependencyInjection;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Extensions;
using Orcus.Administration.Library.Services;
using Orcus.Administration.Library.ViewModels;
using Orcus.Utilities;
using Prism.Commands;
using Tasks.Infrastructure.Administration.Rest.V1;
using Tasks.Infrastructure.Administration.ViewModels.Overview;
using Unclassified.TxLib;

namespace Tasks.Infrastructure.Administration.ViewModels
{
    public class TasksViewModel : OverviewTabBase
    {
        private readonly IRestClient _restClient;
        private readonly IComponentContext _services;
        private readonly IWindowService _windowService;
        private DelegateCommand _createTaskCommand;
        private bool _isTasksLoaded;
        private ObservableCollection<TaskViewModel> _tasks;
        private ICollectionView _tasksView;
        private DelegateCommand<TaskViewModel> _openTaskSessionsCommand;

        public TasksViewModel(IWindowService windowService, IRestClient restClient, IComponentContext services) : base(Tx.T("TasksView:Tasks"),
            PackIconFontAwesomeKind.CalendarCheckRegular)
        {
            _windowService = windowService;
            _restClient = restClient;
            _services = services;
        }

        public ICollectionView TasksView
        {
            get => _tasksView;
            set => SetProperty(ref _tasksView, value);
        }

        public DelegateCommand CreateTaskCommand
        {
            get
            {
                return _createTaskCommand ?? (_createTaskCommand = new DelegateCommand(() =>
                {
                    if (_windowService.ShowDialog<CreateTaskViewModel>() == true)
                        UpdateTasks().Forget();
                }));
            }
        }

        public DelegateCommand<TaskViewModel> OpenTaskSessionsCommand
        {
            get
            {
                return _openTaskSessionsCommand ?? (_openTaskSessionsCommand = new DelegateCommand<TaskViewModel>(async parameter =>
                {
                    var sessions = await TasksResource.GetTaskSessions(parameter.Id, _restClient);

                    var viewModel = _services.Resolve<TaskOverviewViewModel>();
                    viewModel.Initialize(sessions);

                    _windowService.ShowDialog(viewModel);
                }));
            }
        }

        public override void OnActivated()
        {
            base.OnActivated();

            if (!_isTasksLoaded)
            {
                _isTasksLoaded = true;
                UpdateTasks().Forget();
            }
        }

        private async Task UpdateTasks()
        {
            var tasks = await TasksResource.GetTasks(_restClient);
            _tasks = new ObservableCollection<TaskViewModel>(tasks.Select(x => new TaskViewModel(x)));

            TasksView = new ListCollectionView(_tasks);
        }
    }
}