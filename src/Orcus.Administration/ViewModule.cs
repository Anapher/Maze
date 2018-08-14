using Autofac;
using Orcus.Administration.Views.Main;
using Orcus.Administration.Views.Main.Overview;
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

            _regionManager.RegisterViewWithRegion("OverviewTabs", typeof(ClientsView));
            _regionManager.RegisterViewWithRegion("OverviewTabs", typeof(ModulesView));
        }
    }
}