using System;
using System.Threading;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Regions;

namespace Orcus.Administration.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;

        public MainViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }
    }
}
