using System;
using System.Windows.Controls;
using Maze.Administration.Library.Views;

namespace Console.Administration.Resources
{
    public class VisualStudioIcons : ResourceDictionaryProvider
    {
        public VisualStudioIcons() : base(new Uri("/Console.Administration;component/Resources/VisualStudioIcons.xaml", UriKind.Relative))
        {
        }

        public Viewbox Console => GetIcon();
    }
}