using System;
using System.ComponentModel;
using System.Windows;
using Console.Administration.ViewModels;
using Maze.Administration.Library.Views;

namespace Console.Administration.Views
{
    /// <summary>
    ///     Interaction logic for ConsoleView.xaml
    /// </summary>
    public partial class ConsoleView
    {
        public ConsoleView(IShellWindow viewManager) : base(viewManager)
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
                ConsoleControl.ProcessConsole = ((ConsoleViewModel) e.NewValue).ProcessConsole;

            ((ConsoleViewModel) DataContext).PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ConsoleViewModel.ProcessConsole):
                    ConsoleControl.ProcessConsole = ((ConsoleViewModel) sender).ProcessConsole;
                    break;
            }
        }

        private void ConsoleControl_OnExitRequested(object sender, EventArgs e)
        {
            ((ConsoleViewModel) DataContext).ProcessExitedCommand.Execute();
        }
    }
}