using MahApps.Metro.IconPacks;
using Orcus.Administration.Library.ViewModels;
using Unclassified.TxLib;

namespace Orcus.Administration.ViewModels.Overview
{
    public class ClientsViewModel : OverviewTabBase
    {
        public ClientsViewModel() : base(Tx.T("Clients"), PackIconFontAwesomeKind.BarsSolid)
        {
        }
    }
}