using System;
using System.Windows.Controls;
using Maze.Administration.Library.Views;

namespace ClientPanel.Administration.Resources
{
    public class VisualStudioIcons : ResourceDictionaryProvider
    {
        public VisualStudioIcons() : base(new Uri("/ClientPanel.Administration;component/Resources/VisualStudioIcons.xaml", UriKind.Relative))
        {
        }

        public Viewbox CodeDefinitionWindow => GetIcon();
    }
}