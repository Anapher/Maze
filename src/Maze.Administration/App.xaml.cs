using System.Windows;
using Maze.Administration.Views;

namespace Maze.Administration
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            new MainWindow().Show();
        }
    }
}