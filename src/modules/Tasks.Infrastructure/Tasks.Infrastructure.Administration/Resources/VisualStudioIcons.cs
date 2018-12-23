using System;
using System.Windows.Controls;
using Orcus.Administration.Library.Views;

namespace Tasks.Infrastructure.Administration.Resources
{
    public class VisualStudioIcons : ResourceDictionaryProvider
    {
        public VisualStudioIcons() : base(
            new Uri("/Tasks.Infrastructure.Administration;component/Resources/VisualStudioIcons.xaml", UriKind.Relative))
        {
        }

        public Viewbox SelectCommandColumn => GetIcon();
        public Viewbox NewCatalog => GetIcon();
        public Viewbox CatalogPolulation => GetIcon();
    }
}