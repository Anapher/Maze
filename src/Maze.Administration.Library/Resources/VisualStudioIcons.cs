using System;
using System.Windows.Controls;
using Maze.Administration.Library.Views;

namespace Maze.Administration.Library.Resources
{
    internal class VisualStudioIcons : ResourceDictionaryProvider, ILibraryIcons
    {
        public VisualStudioIcons() : base(
            new Uri("/Maze.Administration.Library;component/Resources/VisualStudioIcons.xaml", UriKind.Relative))
        {
        }

        public Viewbox UserVoice => GetIcon();
        public Viewbox ComputerService => GetIcon();
        public Viewbox ComputerSystem => GetIcon();
        public Viewbox HideTimeline => GetIcon();
    }
}