using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Tasks.Infrastructure.Core.Commands;
using Tasks.Infrastructure.Core.Filter;
using Tasks.Infrastructure.Core.StopEvents;
using Tasks.Infrastructure.Core.Triggers;

namespace Tasks.Infrastructure.Core.Tests
{
    public class OperatingSystemFilterInfo : FilterInfo, IXmlSerializable
    {
        public string Min { get; set; }

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            Min = reader.GetAttribute("min");
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("min", Min);
        }
    }

    public class DateTimeTriggerInfo : TriggerInfo, IXmlSerializable
    {
        public DateTimeOffset Date { get; set; }

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            Date = DateTimeOffset.Parse(reader.GetAttribute("date"));
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("date", Date.ToString("O"));
        }
    }

    public class DurationStopEvent : StopEventInfo
    {
        [XmlAttribute("duration")]
        public TimeSpan Duration { get; set; }
    }

    [MazeCommand("Maze.WakeOnLan", "SystemUtilities;TaskManager")]
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
            writer.WriteAttributeString("hash", Hash.ToString());
            writer.WriteString(Content);
        }
    }
    
    public class EmptyCommandInfo : CommandInfo
    {
    }
}