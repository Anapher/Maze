using Prism.Mvvm;
using Prism.Regions;

namespace Maze.Administration.Library.ViewModels
{
    public abstract class ViewModelBase : BindableBase, INavigationAware
    {
        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        public virtual bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
    }
}