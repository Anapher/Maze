using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Moq;
using Orcus.Server.Connection.Tasks;
using Orcus.Server.Connection.Tasks.Audience;
using Orcus.Server.Connection.Tasks.Commands;
using Orcus.Server.Connection.Tasks.Filter;
using Orcus.Server.Connection.Tasks.StopEvents;
using Orcus.Server.Connection.Tasks.Triggers;
using Orcus.Server.Connection.Utilities;
using Xunit;

namespace Orcus.Server.Connection.Tests.Tasks
{
    public class OrcusTaskWriterTests
    {
        private readonly OrcusTask _task;

        public OrcusTaskWriterTests()
        {
            _task = new OrcusTask
            {
                Name = "TestCommand",
                Id = Guid.Parse("53221F85-23DC-4C4C-BD27-A26A5F85BCA0"),
                Audience = new AudienceCollection {IsAll = true, IncludesServer = true},
                Filters = new List<FilterInfo> {new OperatingSystemFilter {Min = "Windows10"}},
                Triggers =
                    new List<TriggerInfo> {new DateTimeTrigger {Date = new DateTimeOffset(2017, 8, 10, 1, 0, 0, TimeSpan.Zero)}},
                StopEvents = new List<StopEventInfo> {new DurationStopEvent {Duration = TimeSpan.FromMinutes(1.36)}},
                Commands = new List<CommandInfo> {new WakeOnLanCommand {Content = "Hello World!!!", Hash = 2845}}
            };
        }

        [Fact]
        public void TestWriteTaskServer()
        {
            var expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<task>
    <metadata>
        <name>TestCommand</name>
        <id>53221f85-23dc-4c4c-bd27-a26a5f85bca0</id>
    </metadata>
    <audience>
        <AllClients />
        <Server />
    </audience>
    <filters>
        <OperatingSystem min=""Windows10"" />
    </filters>
    <triggers>
        <DateTime date=""2017-08-10T01:00:00.0000000+00:00"" />
    </triggers>
    <stop>
        <Duration duration=""PT1M21.6S"" />
    </stop>
    <commands>
        <Command name=""Maze.WakeOnLan"" modules=""SystemUtilities;TaskManager"" hash=""2845"">Hello World!!!</Command>
    </commands>
</task>";
            
            CompareWriteTask(_task, TaskDetails.Server, expected);
        }

        [Fact]
        public void TestWriteTaskClient()
        {
            var expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<task>
    <metadata>
        <name>TestCommand</name>
        <id>53221f85-23dc-4c4c-bd27-a26a5f85bca0</id>
    </metadata>
    <triggers>
        <Idle time=""101"" />
    </triggers>
    <stop>
        <Duration duration=""PT1M21.6S"" />
    </stop>
    <commands>
        <Command name=""Maze.WakeOnLan"" modules=""SystemUtilities;TaskManager"" hash=""2845"">Hello World!!!</Command>
    </commands>
</task>";

            CompareWriteTask(_task, TaskDetails.Client, expected);
        }

        [Fact]
        public void TestWriteTaskExecution()
        {
            var expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<task>
    <metadata>
        <name>TestCommand</name>
        <id>53221f85-23dc-4c4c-bd27-a26a5f85bca0</id>
    </metadata>
    <stop>
        <Duration duration=""PT1M21.6S"" />
    </stop>
    <commands>
        <Command name=""Maze.WakeOnLan"" modules=""SystemUtilities;TaskManager"" hash=""2845"">Hello World!!!</Command>
    </commands>
</task>";

            CompareWriteTask(_task, TaskDetails.Execution, expected);
        }

        private static void CompareWriteTask(OrcusTask orcusTask, TaskDetails details, string expected)
        {
            var mock = new Mock<ITaskComponentResolver>();
            mock.Setup(x => x.ResolveName(It.IsAny<Type>())).Returns<Type>(GetName);

            using (var memoryStream = new MemoryStream())
            {
                var xmlWriter = XmlWriter.Create(memoryStream,
                    new XmlWriterSettings { OmitXmlDeclaration = false, Indent = true, IndentChars = "    ", Encoding = new UTF8Encoding(false) });
                var writer = new OrcusTaskWriter(xmlWriter, mock.Object, new XmlSerializerCache());
                writer.Write(orcusTask, details);

                var result = Encoding.UTF8.GetString(memoryStream.ToArray());
                Assert.Equal(expected, result);
            }
        }

        private static string GetName(Type arg)
        {
            switch (arg)
            {
                case var type when typeof(FilterInfo).IsAssignableFrom(type):
                    return arg.Name.Replace("FilterInfo", null);
                case var type when typeof(TriggerInfo).IsAssignableFrom(type):
                    return arg.Name.Replace("TriggerInfo", null);
                case var type when typeof(StopEventInfo).IsAssignableFrom(type):
                    return arg.Name.Replace("StopEvent", null);
            }

            throw new ArgumentException("Invalid type: " + arg.FullName, nameof(arg));
        }
    }
}