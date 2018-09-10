using System;
using System.Windows.Controls;
using Orcus.Administration.Library.Views;

namespace TaskManager.Administration.Resources
{
    public class VisualStudioIcons : ResourceDictionaryProvider
    {
        public VisualStudioIcons() : base(
            new Uri("/TaskManager.Administration;component/Resources/VisualStudioIcons.xaml", UriKind.Relative))
        {
        }

        public Viewbox Process => GetIcon();
    }
}