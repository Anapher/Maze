using System;
using System.Windows.Controls;
using Orcus.Administration.Library.Views;

namespace UserInteraction.Administration.Resources
{
    public class VisualStudioIcons : ResourceDictionaryProvider
    {
        public VisualStudioIcons() : base(
            new Uri("/UserInteraction.Administration;component/Resources/VisualStudioIcons.xaml", UriKind.Relative))
        {
        }

        public Viewbox MessageBox => GetIcon();
    }
}