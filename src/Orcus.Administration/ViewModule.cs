using Orcus.Administration.Views.Main;
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
            _regionManager.RegisterViewWithRegion("MainContent", typeof(LoginView));
            _regionManager.RegisterViewWithRegion("MainContent", typeof(OverviewView));
        }
    }
}