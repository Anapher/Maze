using System.Security;
using System.Windows.Controls;
using Maze.Administration.ViewModels.Main;

namespace Maze.Administration.Views.Main
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
#if DEBUG
            Loaded += (sender, args) =>
            {
                var loginVm = (LoginViewModel)DataContext;
                loginVm.Username = "test";
                var pw = new SecureString();
                foreach (var c in "test")
                    pw.AppendChar(c);

                Utilities.TaskExtensions.Forget(loginVm.LoginCommand.Execute(pw));
            };
#endif
        }
    }
}
