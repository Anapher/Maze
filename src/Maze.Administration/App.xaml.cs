using System;
using System.Windows;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
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

            using (var stream = GetResourceStream(new Uri("/Resources/SyntaxHighlighting/Json.xshd", UriKind.Relative)).Stream)
            using (var reader = new XmlTextReader(stream))
            {
                HighlightingManager.Instance.RegisterHighlighting("Json", new string[0],
                    HighlightingLoader.Load(reader, HighlightingManager.Instance));
            }

            new MainWindow().Show();
        }
    }
}