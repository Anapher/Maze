using System.ComponentModel;
using System.Windows.Input;

namespace Orcus.Administration.ViewModels.Overview.Modules
{
    public interface IModuleTabViewModel
    {
        ICollectionView Modules { get; }
        ICommand RefreshCommand { get; }
        string SearchText { get; set; }
        bool IncludePrerelease { get; set; }

        void Initialize(IModuleService service);
    }
}