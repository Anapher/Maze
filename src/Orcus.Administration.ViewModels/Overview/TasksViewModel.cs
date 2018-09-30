using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahApps.Metro.IconPacks;
using Orcus.Administration.Library.ViewModels;
using Unclassified.TxLib;

namespace Orcus.Administration.ViewModels.Overview
{
    public class TasksViewModel : OverviewTabBase
    {
        public TasksViewModel() : base(Tx.T("TasksView:Tasks"), PackIconFontAwesomeKind.CalendarCheckRegular)
        {
        }
    }
}
