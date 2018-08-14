using MahApps.Metro.IconPacks;
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