using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace Maze.Administration.Views.Main
{
    public class MainUserControl : UserControl
    {
        public static readonly DependencyProperty RightWindowCommandsProperty = DependencyProperty.Register("RightWindowCommands",
            typeof(WindowCommands), typeof(MainUserControl), new PropertyMetadata(default(WindowCommands)));

        public WindowCommands RightWindowCommands
        {
            get => (WindowCommands) GetValue(RightWindowCommandsProperty);
            set => SetValue(RightWindowCommandsProperty, value);
        }
    }
}