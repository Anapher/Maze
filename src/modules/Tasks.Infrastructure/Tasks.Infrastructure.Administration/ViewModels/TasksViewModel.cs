using MahApps.Metro.IconPacks;
using Orcus.Administration.Library.ViewModels;
using Unclassified.TxLib;

namespace Tasks.Infrastructure.Administration.ViewModels
{
    public class TasksViewModel : OverviewTabBase
    {
        public TasksViewModel() : base(Tx.T("TasksView:Tasks"), PackIconFontAwesomeKind.CalendarCheckRegular)
        {
        }
    }
}
