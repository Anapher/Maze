using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Anapher.Wpf.Toolkit.Windows;
using Maze.Administration.Views.Main;

namespace Maze.Administration.Views
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IWindow
    {
        private MainUserControl _previousMainContent;

        public MainWindow()
        {
            InitializeComponent();
            var descriptor = DependencyPropertyDescriptor.FromProperty(ContentControl.ContentProperty, typeof(ContentControl));
            descriptor.AddValueChanged(MainContentControl, OnContentChanged);
        }

        private void OnContentChanged(object sender, EventArgs e)
        {
            var view = (MainUserControl) MainContentControl.Content;
            if (view == null)
                return;

            if (RightWindowCommands == null)
            {
                RightWindowCommands = view.RightWindowCommands;
            }
            else
            {
                if (_previousMainContent?.RightWindowCommands != null)
                {
                    Move(RightWindowCommands.Items, _previousMainContent.RightWindowCommands.Items);
                }

                RightWindowCommands.Items.Clear();

                if (view.RightWindowCommands != null)
                {
                    foreach (var frameworkElement in view.RightWindowCommands.Items.OfType<FrameworkElement>())
                        frameworkElement.DataContext = view.DataContext;

                    Move(view.RightWindowCommands.Items, RightWindowCommands.Items);
                }

                _previousMainContent = view;
            }
        }

        private void Move(ItemCollection source, ItemCollection target)
        {
            var items = source.Cast<object>().ToList();
            source.Clear();

            foreach (var item in items)
                target.Add(item);
        }
    }
}