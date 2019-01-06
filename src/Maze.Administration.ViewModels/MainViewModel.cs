using Prism.Mvvm;
using Prism.Regions;

namespace Maze.Administration.ViewModels
{
    public class MainViewModel : BindableBase
    {
        public MainViewModel(IRegionNavigationService region)
        {
            region.RequestNavigate();
        }
    }
}
