using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using Maze.Administration.ViewModels;
using Microsoft.Extensions.Logging;

namespace Maze.Administration.Views
{
    /// <summary>
    ///     Interaction logic for DeployClientBuildView.xaml
    /// </summary>
    public partial class DeployClientBuildView : ILogger
    {
        private Paragraph _paragraph;

        private readonly Dictionary<LogLevel, Brush> _logLevelBrushes = new Dictionary<LogLevel, Brush>
        {
            {LogLevel.Debug, new SolidColorBrush(Color.FromRgb(196, 196, 196))},
            {LogLevel.Information, Brushes.White},
            {LogLevel.Warning, new SolidColorBrush(Color.FromRgb(230, 126, 34))},
            {LogLevel.Error, new SolidColorBrush(Color.FromRgb(231, 76, 60))},
            {LogLevel.Critical, new SolidColorBrush(Color.FromRgb(241, 196, 15))},
            {LogLevel.None, new SolidColorBrush(Color.FromRgb(46, 204, 113))},
            {LogLevel.Trace, new SolidColorBrush(Color.FromRgb(22, 160, 133))},
        };

        private readonly Brush _timeBrush = new SolidColorBrush(Color.FromRgb(243, 156, 18));

        public DeployClientBuildView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is DeployClientBuildViewModel viewModel)
            {
                _paragraph = new Paragraph();
                LogRichTextBox.Document.Blocks.Clear();
                LogRichTextBox.Document.Blocks.Add(_paragraph);

                viewModel.LoggerPresenter = this;
            }
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                var brush = _logLevelBrushes[logLevel];
                var prefix = logLevel.ToString().ToUpper();
                var now = DateTimeOffset.Now;

                _paragraph.Inlines.Add(new Run(now.ToString("HH:mm:ss.ff")) {Foreground = _timeBrush});
                _paragraph.Inlines.Add(new Run($" [{prefix}] {formatter(state, exception)}") { Foreground = brush });
                _paragraph.Inlines.Add(new LineBreak());

                LogRichTextBox.ScrollToEnd();
            }));
        }

        bool ILogger.IsEnabled(LogLevel logLevel) => throw new NotSupportedException();
        IDisposable ILogger.BeginScope<TState>(TState state) => throw new NotSupportedException();
    }
}