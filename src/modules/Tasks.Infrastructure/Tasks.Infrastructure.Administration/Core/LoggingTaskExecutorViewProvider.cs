using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog.Events;
using Serilog.Parsing;
using Tasks.Infrastructure.Administration.Library.Result;

namespace Tasks.Infrastructure.Administration.Core
{
    public class LoggingTaskExecutorViewProvider : ICommandResultViewProvider
    {
        public int Priority { get; set; } = 5;

        public UIElement GetView(HttpResponseMessage responseMessage, IComponentContext context)
        {
            if (responseMessage.Content.Headers.ContentType.MediaType != "orcus/jsonlog")
                return null;

            return null;
        }
    }

    public class LogEventJsonConverter : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);

            DateTimeOffset timestamp = serializer.Deserialize<DateTimeOffset>(jObject["Timestamp"].CreateReader());
            LogEventLevel level = serializer.Deserialize<LogEventLevel>(jObject["Level"].CreateReader());
            Exception exception = serializer.Deserialize<Exception>(jObject["Exception"].CreateReader());
            string messageTemplateText = jObject["MessageTemplate"]["Text"].Value<string>();
            MessageTemplateParser messageTemplateParser = new MessageTemplateParser();
            MessageTemplate messageTemplate = messageTemplateParser.Parse(messageTemplateText);
            Dictionary<string, LogEventPropertyValue> logEventPropertyValues = serializer.Deserialize<Dictionary<string, LogEventPropertyValue>>(jObject["Properties"].CreateReader());

            return new LogEvent(timestamp, level, exception, messageTemplate, logEventPropertyValues.Select(x => new LogEventProperty(x.Key, x.Value)));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(LogEvent).IsAssignableFrom(objectType);
        }
    }
}
