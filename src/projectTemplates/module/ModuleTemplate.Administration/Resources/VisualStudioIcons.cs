using System;
using System.Windows.Controls;
using Maze.Administration.Library.Views;

namespace ModuleTemplate.Administration.Resources
{
    public class VisualStudioIcons : ResourceDictionaryProvider
    {
        public VisualStudioIcons() : base(new Uri("/ModuleNamePlaceholder.Administration;component/Resources/VisualStudioIcons.xaml", UriKind.Relative))
        {
        }

        public Viewbox Icon => GetIcon();
    }
}