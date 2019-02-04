using System.Security;
using Maze.Administration.ViewModels.Main;
using Maze.Utilities;

namespace Maze.Administration.Views.Main
{
    /// <summary>
    ///     Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView
    {
        public LoginView()
        {
            InitializeComponent();
#if DEBUG
            Loaded += (sender, args) =>
            {
                var loginVm = (LoginViewModel) DataContext;
                loginVm.Username = "test";
                var pw = new SecureString();
                foreach (var c in "test")
                    pw.AppendChar(c);

                TaskExtensions.Forget(loginVm.LoginCommand.Execute(pw));
            };
#endif
        }
    }
}