using Serilog.Events;
using Serilog.Formatting.Display;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Unclassified.TxLib;

namespace Tasks.Infrastructure.Administration.Views.TaskOverview.ResultView
{
    /// <summary>
    /// Interaction logic for LogView.xaml
    /// </summary>
    public partial class LogView : UserControl
    {
        public LogView(IEnumerable<LogEvent> logEvents)
        {
            InitializeComponent();

            var levelStyles = new Dictionary<LogEventLevel, Func<Run>>
            {
                {LogEventLevel.Verbose, () => new Run{Foreground = new SolidColorBrush(Color.FromArgb(125, 255, 255, 255)), FontStyle = FontStyles.Italic } },
                {LogEventLevel.Debug, () => new Run{Foreground = new SolidColorBrush(Color.FromArgb(125, 255, 255, 255)) } },
                {LogEventLevel.Information, () => new Run{Foreground = Brushes.White } },
                {LogEventLevel.Warning, () => new Run{Foreground = new SolidColorBrush(Color.FromRgb(230, 126, 32)) } },
                {LogEventLevel.Error, () => new Run{Foreground = new SolidColorBrush(Color.FromRgb(231, 76, 60)) } },
                {LogEventLevel.Fatal, () => new Run{Background = new SolidColorBrush(Color.FromRgb(231, 76, 60)), Foreground = Brushes.White } }
            };
            var paragraph = new Paragraph();

            var stringBuilder = new StringBuilder();
            var logWriter = new StringWriter(stringBuilder);

            var formatter = new MessageTemplateTextFormatter("{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}", CultureInfo.GetCultureInfo(Tx.CurrentThreadCulture));
            foreach (var logEvent in logEvents)
            {
                formatter.Format(logEvent, logWriter);

                var run = levelStyles[logEvent.Level]();
                run.Text = stringBuilder.ToString();

                paragraph.Inlines.Add(run);

                stringBuilder.Clear();
            }

            RichTextBox logOutput = this.LogTextBox;
            logOutput.Document = new FlowDocument();
            logOutput.Document.Blocks.Add(paragraph);
        }
    }
}
