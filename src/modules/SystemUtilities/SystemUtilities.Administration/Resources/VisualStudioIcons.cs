using System;
using System.Windows.Controls;
using Maze.Administration.Library.Views;

namespace SystemUtilities.Administration.Resources
{
    public class VisualStudioIcons : ResourceDictionaryProvider
    {
        public VisualStudioIcons() : base(new Uri("/SystemUtilities.Administration;component/Resources/VisualStudioIcons.xaml", UriKind.Relative))
        {
        }

        public Viewbox Icon => GetIcon();
    }
}