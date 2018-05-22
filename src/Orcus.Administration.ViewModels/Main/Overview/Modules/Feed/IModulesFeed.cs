using System.ComponentModel;
using System.Windows.Input;

namespace Orcus.Administration.ViewModels.Main.Overview.Modules.Feed
{
    public interface IModulesFeed
    {
        string SearchText { get; set; }
        ICollectionView View { get; }
        ICommand RefreshViewCommand { get; }
        bool IncludePrerelease { get; set; }
    }
}
