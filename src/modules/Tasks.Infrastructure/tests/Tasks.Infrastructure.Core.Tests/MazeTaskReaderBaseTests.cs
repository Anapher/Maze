using System;
using System.Xml.Linq;
using Xunit;

namespace Tasks.Infrastructure.Core.Tests
{
    public class MazeTaskReaderBaseTests
    {
        private class ReaderImpl : MazeTaskReaderBase
        {
            public ReaderImpl(XDocument xml) : base(xml)
            {
            }
        }

        [Fact]
        public void TestDeserialize()
        {
            var test =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?><task>    <metadata>        <name>Hello World</name>        <id>0d852117-6adf-4af7-8c8b-f52300ccae15</id>        <!-- <isActive>true</isActive> that must be defined in the server and should not be part of the task -->    </metadata>    <audience>        <all />        <client id=\"1\" />        <group id=\"testGroup\" />    </audience>    <conditions>        <OperatingSystem min=\"WinVista\" />    </conditions>    <transmission>        <DateTime date=\"\" />    </transmission>    <execution>        <IdleExecutionEvent idleTime=\"120\" />    </execution>    <stop>        <Duration time=\"00:01:00\" />        <DateTime date=\"\" />    </stop>    <commands>        <command name=\"Maze.WakeOnLan\" modules=\"SystemUtilties;TaskManager\">            asdasdadsdas        </command>    </commands></task>";


            var reader = new ReaderImpl(XDocument.Parse(test));
            Assert.Equal("Hello World", reader.GetName());
            Assert.Equal(Guid.Parse("0d852117-6adf-4af7-8c8b-f52300ccae15"), reader.GetId());
        }
    }
}