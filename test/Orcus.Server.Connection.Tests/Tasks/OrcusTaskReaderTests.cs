using System;
using System.Linq;
using System.Xml.Linq;
using Moq;
using Orcus.Server.Connection.Commanding;
using Orcus.Server.Connection.Tasks;
using Orcus.Server.Connection.Utilities;
using Xunit;

namespace Orcus.Server.Connection.Tests.Tasks
{
    public class OrcusTaskReaderTests
    {
        public OrcusTaskReaderTests()
        {
            _serializerCache = new XmlSerializerCache();
        }

        private readonly IXmlSerializerCache _serializerCache;

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
        <AllClients />
        <Server />
    </audience>
</task>";


            var reader = new OrcusTaskReader(XDocument.Parse(test), null, _serializerCache);

            var audienceCollection = reader.GetAudience();
            Assert.True(audienceCollection.IsAll);
            Assert.True(audienceCollection.IncludesServer);
            Assert.Empty(audienceCollection);
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


            var reader = new OrcusTaskReader(XDocument.Parse(test), null, _serializerCache);

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


            var reader = new OrcusTaskReader(XDocument.Parse(test), null, _serializerCache);

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
        public void TestCommandMetadata()
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


            var reader = new OrcusTaskReader(XDocument.Parse(test), null, _serializerCache);
            var elements = reader.GetCommandMetadata().ToList();
            Assert.Collection(elements, command =>
            {
                Assert.Equal("Maze.WakeOnLan", command.Name);
                Assert.Collection(command.Modules, s => Assert.Equal("SystemUtilities", s), s => Assert.Equal("TaskManager", s));
            });
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

            var reader = new OrcusTaskReader(XDocument.Parse(test), resolverMock.Object, _serializerCache);
            var elements = reader.GetCommands().ToList();
            Assert.Collection(elements, command =>
            {
                var wolCmd = Assert.IsType<WakeOnLanCommand>(command);
                Assert.Equal("asdasdadsdas", wolCmd.Content);
            });
        }

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
        <AllClients />
    </audience>
</task>";


            var reader = new OrcusTaskReader(XDocument.Parse(test), null, _serializerCache);

            var audienceCollection = reader.GetAudience();
            Assert.True(audienceCollection.IsAll);
            Assert.False(audienceCollection.IncludesServer);
            Assert.Empty(audienceCollection);
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
            resolverMock.Setup(x => x.ResolveExecutionInfo("Idle")).Returns(typeof(IdleExecution));

            var reader = new OrcusTaskReader(XDocument.Parse(test), resolverMock.Object, _serializerCache);
            var elements = reader.GetExecutionEvents().ToList();
            Assert.Collection(elements, info => Assert.Equal(120, Assert.IsType<IdleExecution>(info).Idle));
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
            resolverMock.Setup(x => x.ResolveFilter("OperatingSystem")).Returns(typeof(OperatingSystemFilter));

            var reader = new OrcusTaskReader(XDocument.Parse(test), resolverMock.Object, _serializerCache);
            var elements = reader.GetFilters().ToList();
            Assert.Collection(elements, info => Assert.Equal("Windows7", Assert.IsType<OperatingSystemFilter>(info).Min));
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

            var reader = new OrcusTaskReader(XDocument.Parse(test), resolverMock.Object, _serializerCache);
            var elements = reader.GetTransmissionEvents().ToList();
            Assert.Collection(elements,
                info => Assert.Equal(DateTimeOffset.Parse("2018-10-05T18:21:07.8601530Z"), Assert.IsType<DateTimeTransmission>(info).Date));
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

            var reader = new OrcusTaskReader(XDocument.Parse(test), resolverMock.Object, _serializerCache);
            var elements = reader.GetStopEvents().ToList();
            Assert.Collection(elements, info => Assert.Equal(TimeSpan.FromSeconds(60), Assert.IsType<DurationStopEvent>(info).Duration));
        }
    }
}