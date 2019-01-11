using System;
using System.Windows;
using Maze.Administration.ViewModels.Main;

namespace Maze.Administration.Views.Main
{
    /// <summary>
    ///     Interaction logic for ClientConfigurationView.xaml
    /// </summary>
    public partial class ClientConfigurationView
    {
        public ClientConfigurationView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is ClientConfigurationViewModel oldVm)
                oldVm.OkCommand.CanExecuteChanged -= OkCommandOnCanExecuteChanged;

            var viewModel = (ClientConfigurationViewModel) e.NewValue;
            viewModel.OkCommand.CanExecuteChanged += OkCommandOnCanExecuteChanged;
            OkCommandOnCanExecuteChanged(this, EventArgs.Empty);

            TextEditor.Text = viewModel.Content;
        }

        private void OkCommandOnCanExecuteChanged(object sender, EventArgs e)
        {
            OkButton.IsEnabled = ((ClientConfigurationViewModel) DataContext).OkCommand.CanExecute(TextEditor.Text);
        }

        private void OkButtonOnClick(object sender, RoutedEventArgs e)
        {
            var viewModel = (ClientConfigurationViewModel) DataContext;
            if (viewModel.OkCommand.CanExecute(TextEditor.Text))
                viewModel.OkCommand.Execute(TextEditor.Text);
        }
    }
}