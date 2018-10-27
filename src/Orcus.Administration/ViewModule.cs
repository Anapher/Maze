using Orcus.Administration.Library;
using Orcus.Administration.Views.Main;
using Orcus.Administration.Views.Main.Overview;
using Orcus.Administration.Views.Main.Overview.Clients;
using Prism.Modularity;
using Prism.Regions;

namespace Orcus.Administration
{
    public class ViewModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public ViewModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion("MainContent", typeof(OverviewView));

            _regionManager.RegisterViewWithRegion(RegionNames.OverviewTabs, typeof(ClientsView));
            _regionManager.RegisterViewWithRegion(RegionNames.OverviewTabs, typeof(ModulesView));

            _regionManager.RegisterViewWithRegion(RegionNames.ClientListTabs, typeof(DefaultClientListView));
        }
    }
}