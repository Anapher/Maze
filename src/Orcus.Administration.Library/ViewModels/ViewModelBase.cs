using Prism.Mvvm;
using Prism.Regions;

namespace Orcus.Administration.Library.ViewModels
{
    public abstract class ViewModelBase : BindableBase
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