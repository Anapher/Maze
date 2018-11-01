using MahApps.Metro.IconPacks;
using Orcus.Administration.Library.Extensions;
using Orcus.Administration.Library.Services;
using Orcus.Administration.Library.ViewModels;
using Prism.Commands;
using Unclassified.TxLib;

namespace Tasks.Infrastructure.Administration.ViewModels
{
    public class TasksViewModel : OverviewTabBase
    {
        private readonly IWindowService _windowService;

        public TasksViewModel(IWindowService windowService) : base(Tx.T("TasksView:Tasks"), PackIconFontAwesomeKind.CalendarCheckRegular)
        {
            _windowService = windowService;
        }

        private DelegateCommand _createTaskCommand;

        public DelegateCommand CreateTaskCommand
        {
            get
            {
                return _createTaskCommand ?? (_createTaskCommand = new DelegateCommand(() =>
                {
                    _windowService.ShowDialog<CreateTaskViewModel>();
                }));
            }
        }
    }
}
