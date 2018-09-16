using System;
using System.Windows.Controls;
using Orcus.Administration.Library.Views;

namespace RemoteDesktop.Administration.Resources
{
    public class VisualStudioIcons : ResourceDictionaryProvider
    {
        public VisualStudioIcons() : base(new Uri("/RemoteDesktop.Administration;component/Resources/VisualStudioIcons.xaml", UriKind.Relative))
        {
        }

        public Viewbox Icon => GetIcon();
    }
}