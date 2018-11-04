using Orcus.Administration.Library;
using Prism.Modularity;
using Prism.Regions;
using Tasks.Infrastructure.Administration.Views;
using Unclassified.TxLib;

namespace Tasks.Infrastructure.Administration.Hooks
{
    public class PrismModule : IModule
    {
        public const string ModuleName = "Tasks.Infrastructure";

        private readonly IRegionManager _regionManager;

        public PrismModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.OverviewTabs, typeof(TasksView));
            Tx.LoadFromEmbeddedResource("Tasks.Infrastructure.Administration.Resources.Tasks.Infrastructure.Translation.txd");
        }
    }
}