using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Orcus.Administration.ViewModels.Overview.Modules
{
    public class UpdatesTabViewModel : IModuleTabViewModel
    {
        private ICollectionView _modules;
        public ObservableCollection<ModuleViewModel> Modules { get; }

        ICollectionView IModuleTabViewModel.Modules => _modules;

        public ICommand RefreshCommand { get; }
        public string SearchText { get; set; }
        public bool IncludePrerelease { get; set; }

        public void Initialize(IModuleService service)
        {
        }
    }
}