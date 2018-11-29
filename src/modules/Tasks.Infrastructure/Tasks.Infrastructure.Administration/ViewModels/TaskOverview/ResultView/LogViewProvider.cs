using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Windows;
using Autofac;
using Serilog.Events;
using Serilog.Formatting.Compact.Reader;
using Tasks.Infrastructure.Administration.Library.Result;
using Tasks.Infrastructure.Administration.Views.TaskOverview.ResultView;
using Tasks.Infrastructure.Core.Dtos;

namespace Tasks.Infrastructure.Administration.ViewModels.TaskOverview.ResultView
{
    public class LogViewProvider : ICommandResultViewProvider
    {
        public int Priority { get; set; } = 5;

        public UIElement GetView(HttpResponseMessage responseMessage, CommandResultDto dto, IComponentContext context)
        {
            if (responseMessage.Content.Headers.ContentType.MediaType != "orcus/jsonlog")
                return null;

            var log = responseMessage.Content.ReadAsStringAsync().Result;
            var logEvents = new List<LogEvent>();

            using (var stringReader = new StringReader(log))
            {
                var logReader = new LogEventReader(stringReader);
                while (logReader.TryRead(out var logEvent))
                {
                    logEvents.Add(logEvent);
                }
            }

            return new LogView(logEvents);
        }
    }
}
