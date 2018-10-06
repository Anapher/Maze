using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Moq;
using Orcus.Server.Connection.Tasks;
using Orcus.Server.Connection.Tasks.Audience;
using Orcus.Server.Connection.Tasks.Commands;
using Orcus.Server.Connection.Tasks.Conditions;
using Orcus.Server.Connection.Tasks.Execution;
using Orcus.Server.Connection.Tasks.StopEvents;
using Orcus.Server.Connection.Tasks.Transmission;
using Orcus.Server.Connection.Utilities;
using Xunit;

namespace Orcus.Server.Connection.Tests.Tasks
{
   public class OrcusTaskWriterTests
    {
        [Fact]
        public void TestWriteTask()
        {
            var task = new OrcusTask
            {
                Name = "TestCommand",
                Id = Guid.Parse("53221F85-23DC-4C4C-BD27-A26A5F85BCA0"),
                Audience = new AudienceCollection {IsAll = true, IncludesServer = true},
                Conditions = new List<ConditionInfo> {new OperatingSystemCondition {Min = "Windows10"}},
                Transmission =
                    new List<TransmissionInfo> {new DateTimeTransmission {Date = new DateTimeOffset(2017, 8, 10, 1, 0, 0, TimeSpan.Zero)}},
                Execution = new List<ExecutionInfo> {new IdleExecution {Idle = 101}},
                StopEvents = new List<StopEventInfo> {new DurationStopEvent {Duration = TimeSpan.FromMinutes(1.36)}},
                Commands = new List<CommandInfo> {new WakeOnLanCommand {Content = "Hello World!!!", Hash = 2845}}
            };

            var mock = new Mock<ITaskComponentResolver>();
            mock.Setup(x => x.ResolveName(It.IsAny<Type>())).Returns<Type>(GetName);

            using (var memoryStream = new MemoryStream())
            {
                var xmlWriter = XmlWriter.Create(memoryStream,
                    new XmlWriterSettings {OmitXmlDeclaration = false, Indent = true, IndentChars = "    ", Encoding = new UTF8Encoding(false)});
                var writer = new OrcusTaskWriter(xmlWriter, mock.Object, new XmlSerializerCache());
                writer.Write(task);

                var result = Encoding.UTF8.GetString(memoryStream.ToArray());

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
    <conditions>
        <OperatingSystem min=""Windows10"" />
    </conditions>
    <transmission>
        <DateTime date=""2017-08-10T01:00:00.0000000+00:00"" />
    </transmission>
    <execution>
        <Idle time=""101"" />
    </execution>
    <stop>
        <Duration duration=""PT1M21.6S"" />
    </stop>
    <commands>
        <Command name=""Maze.WakeOnLan"" modules=""SystemUtilities;TaskManager"" hash=""2845"">Hello World!!!</Command>
    </commands>
</task>";

                Assert.Equal(expected, result);
            }
            
        }

        private string GetName(Type arg)
        {
            switch (arg)
            {
                case var type when typeof(ExecutionInfo).IsAssignableFrom(type):
                    return arg.Name.Replace("Execution", null);
                case var type when typeof(ConditionInfo).IsAssignableFrom(type):
                    return arg.Name.Replace("Condition", null);
                case var type when typeof(TransmissionInfo).IsAssignableFrom(type):
                    return arg.Name.Replace("Transmission", null);
                case var type when typeof(StopEventInfo).IsAssignableFrom(type):
                    return arg.Name.Replace("StopEvent", null);
            }

            throw new ArgumentException("Invalid type: " + arg.FullName, nameof(arg));
        }
    }
}
