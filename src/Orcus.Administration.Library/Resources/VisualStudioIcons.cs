using System;
using System.Windows.Controls;
using Orcus.Administration.Library.Views;

namespace Orcus.Administration.Library.Resources
{
    internal class VisualStudioIcons : ResourceDictionaryProvider, ILibraryIcons
    {
        public VisualStudioIcons() : base(
            new Uri("/Orcus.Administration.Library;component/Resources/VisualStudioIcons.xaml", UriKind.Relative))
        {
        }

        public Viewbox UserVoice => GetIcon();
        public Viewbox ComputerService => GetIcon();
    }
}