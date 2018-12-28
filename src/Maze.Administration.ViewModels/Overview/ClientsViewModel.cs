using MahApps.Metro.IconPacks;
using Maze.Administration.Library.ViewModels;
using Unclassified.TxLib;

namespace Maze.Administration.ViewModels.Overview
{
    public class ClientsViewModel : OverviewTabBase
    {
        public ClientsViewModel() : base(Tx.T("Clients"), PackIconFontAwesomeKind.BarsSolid)
        {
        }
    }
}