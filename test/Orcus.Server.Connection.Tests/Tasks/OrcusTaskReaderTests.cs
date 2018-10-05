using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Moq;
using Orcus.Server.Connection.Commanding;
using Orcus.Server.Connection.Tasks;
using Orcus.Server.Connection.Tasks.Commands;
using Orcus.Server.Connection.Tasks.Conditions;
using Orcus.Server.Connection.Tasks.Execution;
using Orcus.Server.Connection.Tasks.StopEvents;
using Orcus.Server.Connection.Tasks.Transmission;
using Xunit;

namespace Orcus.Server.Connection.Tests.Tasks
{
    public class OrcusTaskReaderTests
    {
        [Fact]
        public void TestDeserializeAudienceAll()
        {
            var test = @"<?xml version=""1.0"" encoding=""utf-8""?>
<task>
    <metadata>
        <name>Hello World</name>
        <id>0d852117-6adf-4af7-8c8b-f52300ccae15</id>
    </metadata>
    <audience>
        <All />
    </audience>
</task>";


            var reader = new OrcusTaskReader(XDocument.Parse(test), null);

            var audienceCollection = reader.GetAudience();
            Assert.True(audienceCollection.IsAll);
            Assert.False(audienceCollection.IncludesServer);
            Assert.Empty(audienceCollection);
        }

        [Fact]
        public void TestAudienceAllIncludingServer()
        {
            var test = @"<?xml version=""1.0"" encoding=""utf-8""?>
<task>
    <metadata>
        <name>Hello World</name>
        <id>0d852117-6adf-4af7-8c8b-f52300ccae15</id>
    </metadata>
    <audience>
        <All />
        <Server />
    </audience>
</task>";


            var reader = new OrcusTaskReader(XDocument.Parse(test), null);

            var audienceCollection = reader.GetAudience();
            Assert.True(audienceCollection.IsAll);
            Assert.True(audienceCollection.IncludesServer);
            Assert.Empty(audienceCollection);
        }

        [Fact]
        public void TestAudienceClientsIncludingServer()
        {
            var test = @"<?xml version=""1.0"" encoding=""utf-8""?>
<task>
    <metadata>
        <name>Hello World</name>
        <id>0d852117-6adf-4af7-8c8b-f52300ccae15</id>
    </metadata>
    <audience>
        <Clients id=""C1-10,G5"" />
        <Server />
    </audience>
</task>";


            var reader = new OrcusTaskReader(XDocument.Parse(test), null);

            var audienceCollection = reader.GetAudience();
            Assert.False(audienceCollection.IsAll);
            Assert.True(audienceCollection.IncludesServer);
            Assert.Collection(audienceCollection, target =>
            {
                Assert.Equal(CommandTargetType.Client, target.Type);
                Assert.Equal(1, target.From);
                Assert.Equal(10, target.To);
            }, target =>
            {
                Assert.Equal(CommandTargetType.Group, target.Type);
                Assert.Equal(5, target.From);
                Assert.Equal(5, target.To);
            });
        }

        [Fact]
        public void TestAudienceClients()
        {
            var test = @"<?xml version=""1.0"" encoding=""utf-8""?>
<task>
    <metadata>
        <name>Hello World</name>
        <id>0d852117-6adf-4af7-8c8b-f52300ccae15</id>
    </metadata>
    <audience>
        <Clients id=""C1-10,G5"" />
    </audience>
</task>";


            var reader = new OrcusTaskReader(XDocument.Parse(test), null);

            var audienceCollection = reader.GetAudience();
            Assert.False(audienceCollection.IsAll);
            Assert.False(audienceCollection.IncludesServer);
            Assert.Collection(audienceCollection, target =>
            {
                Assert.Equal(CommandTargetType.Client, target.Type);
                Assert.Equal(1, target.From);
                Assert.Equal(10, target.To);
            }, target =>
            {
                Assert.Equal(CommandTargetType.Group, target.Type);
                Assert.Equal(5, target.From);
                Assert.Equal(5, target.To);
            });
        }

        [Fact]
        public void TestParseConditions()
        {
            var test = @"<?xml version=""1.0"" encoding=""utf-8""?>
<task>
    <metadata>
        <name>Hello World</name>
        <id>0d852117-6adf-4af7-8c8b-f52300ccae15</id>
    </metadata>
    <conditions>
        <OperatingSystem min=""Windows7"" />
    </conditions>
</task>";

            var resolverMock = new Mock<ITaskComponentResolver>();
            resolverMock.Setup(x => x.ResolveCondition("OperatingSystem")).Returns(typeof(OperatingSystemCondition));

            var reader = new OrcusTaskReader(XDocument.Parse(test), resolverMock.Object);
            var elements = reader.GetConditions().ToList();
            Assert.Collection(elements, info => Assert.Equal("Windows7", Assert.IsType<OperatingSystemCondition>(info).Min));
        }

        public class OperatingSystemCondition : ConditionInfo, IXmlSerializable
        {
            public string Min { get; set; }

            public XmlSchema GetSchema() => null;

            public void ReadXml(XmlReader reader)
            {
                Min = reader.GetAttribute("min");
            }

            public void WriteXml(XmlWriter writer)
            {
                throw new NotSupportedException();
            }
        }

        [Fact]
        public void TestParseTransmissionEvents()
        {
            var test = @"<?xml version=""1.0"" encoding=""utf-8""?>
<task>
    <metadata>
        <name>Hello World</name>
        <id>0d852117-6adf-4af7-8c8b-f52300ccae15</id>
    </metadata>
    <transmission>
        <DateTime date=""2018-10-05T18:21:07.8601530Z"" />
    </transmission>
</task>";

            var resolverMock = new Mock<ITaskComponentResolver>();
            resolverMock.Setup(x => x.ResolveTransmissionInfo("DateTime")).Returns(typeof(DateTimeTransmission));

            var reader = new OrcusTaskReader(XDocument.Parse(test), resolverMock.Object);
            var elements = reader.GetTransmissionEvents().ToList();
            Assert.Collection(elements, info => Assert.Equal(DateTimeOffset.Parse("2018-10-05T18:21:07.8601530Z"), Assert.IsType<DateTimeTransmission>(info).Date));
        }

        public class DateTimeTransmission : TransmissionInfo, IXmlSerializable
        {
            public DateTimeOffset Date { get; set; }

            public XmlSchema GetSchema() => null;

            public void ReadXml(XmlReader reader)
            {
                Date = DateTimeOffset.Parse(reader.GetAttribute("date"));
            }

            public void WriteXml(XmlWriter writer)
            {
                throw new NotSupportedException();
            }
        }

        [Fact]
        public void TestExecutionEvents()
        {
            var test = @"<?xml version=""1.0"" encoding=""utf-8""?>
<task>
    <metadata>
        <name>Hello World</name>
        <id>0d852117-6adf-4af7-8c8b-f52300ccae15</id>
    </metadata>
     <execution>
        <Idle time=""120"" />
    </execution>
</task>";

            var resolverMock = new Mock<ITaskComponentResolver>();
            resolverMock.Setup(x => x.ResolveExecutionInfo("Idle")).Returns(typeof(IdleExecutionInfo));

            var reader = new OrcusTaskReader(XDocument.Parse(test), resolverMock.Object);
            var elements = reader.GetExecutionEvents().ToList();
            Assert.Collection(elements, info => Assert.Equal(120, Assert.IsType<IdleExecutionInfo>(info).Idle));
        }

        public class IdleExecutionInfo : ExecutionInfo
        {
            [XmlAttribute("time")]
            public int Idle { get; set; }
        }

        [Fact]
        public void TestStopEvent()
        {
            var test = @"<?xml version=""1.0"" encoding=""utf-8""?>
<task>
    <metadata>
        <name>Hello World</name>
        <id>0d852117-6adf-4af7-8c8b-f52300ccae15</id>
    </metadata>
     <stop>
        <Duration duration=""PT1M"" />
    </stop>
</task>";

            var resolverMock = new Mock<ITaskComponentResolver>();
            resolverMock.Setup(x => x.ResolveStopEvent("Duration")).Returns(typeof(DurationStopEvent));

            var reader = new OrcusTaskReader(XDocument.Parse(test), resolverMock.Object);
            var elements = reader.GetStopEvents().ToList();
            Assert.Collection(elements, info => Assert.Equal(TimeSpan.FromSeconds(60), Assert.IsType<DurationStopEvent>(info).Duration));
        }

        public class DurationStopEvent : StopEventInfo
        {
            [XmlAttribute("duration")]
            public TimeSpan Duration { get; set; }
        }

        [Fact]
        public void TestCommands()
        {
            var test = @"<?xml version=""1.0"" encoding=""utf-8""?>
<task>
    <metadata>
        <name>Hello World</name>
        <id>0d852117-6adf-4af7-8c8b-f52300ccae15</id>
    </metadata>
     <commands>
        <Command name=""Maze.WakeOnLan"" modules=""SystemUtilities;TaskManager"" hash=""12"">
            asdasdadsdas
        </Command>
    </commands>
</task>";

            var resolverMock = new Mock<ITaskComponentResolver>();
            resolverMock.Setup(x => x.ResolveCommand("Maze.WakeOnLan")).Returns(typeof(WakeOnLanCommand));

            var reader = new OrcusTaskReader(XDocument.Parse(test), resolverMock.Object);
            var elements = reader.GetCommands().ToList();
            Assert.Collection(elements, command =>
            {
                var wolCmd = Assert.IsType<WakeOnLanCommand>(command);
                Assert.Equal("Maze.WakeOnLan", wolCmd.Name);
                Assert.Collection(wolCmd.Modules, s => Assert.Equal("SystemUtilities", s), s => Assert.Equal("TaskManager", s));
                Assert.Equal("asdasdadsdas", wolCmd.Content);
            });
        }

        public class WakeOnLanCommand : CommandInfo, IXmlSerializable
        {
            public string Content { get; set; }
            public int Hash { get; set; }   

            public XmlSchema GetSchema() => null;

            public void ReadXml(XmlReader reader)
            {
                Hash = int.Parse(reader.GetAttribute("hash"));
                reader.Read();
                Content = reader.ReadContentAsString().Trim();
            }

            public void WriteXml(XmlWriter writer)
            {
                throw new NotSupportedException();
            }
        }
    }
}