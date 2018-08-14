using MahApps.Metro.IconPacks;
using Prism.Mvvm;
using Prism.Regions;

namespace Orcus.Administration.ViewModels.Overview
{
    public class OverviewTabBase : BindableBase, INavigationAware
    {
        public OverviewTabBase(string title, PackIconFontAwesomeKind icon)
        {
            Title = title;
            Icon = icon;
        }

        public string Title { get; }
        public PackIconFontAwesomeKind Icon { get; }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        public virtual bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
    }
}