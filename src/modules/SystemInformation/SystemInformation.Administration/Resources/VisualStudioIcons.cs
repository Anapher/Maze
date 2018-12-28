using System;
using System.Windows.Controls;
using Maze.Administration.Library.Views;

namespace SystemInformation.Administration.Resources
{
    public class VisualStudioIcons : ResourceDictionaryProvider
    {
        public VisualStudioIcons() : base(new Uri("/SystemInformation.Administration;component/Resources/VisualStudioIcons.xaml", UriKind.Relative))
        {
        }

        public Viewbox SystemInfo => GetIcon();
    }
}