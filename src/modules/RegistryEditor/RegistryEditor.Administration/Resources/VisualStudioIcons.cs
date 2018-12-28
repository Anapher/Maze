using System;
using System.Windows.Controls;
using Maze.Administration.Library.Views;

namespace RegistryEditor.Administration.Resources
{
    public class VisualStudioIcons : ResourceDictionaryProvider
    {
        public VisualStudioIcons() : base(new Uri("/RegistryEditor.Administration;component/Resources/VisualStudioIcons.xaml", UriKind.Relative))
        {
        }

        public Viewbox Registry => GetIcon();
        public Viewbox NewSolutionFolder => GetIcon();
        public Viewbox Edit => GetIcon();
    }
}