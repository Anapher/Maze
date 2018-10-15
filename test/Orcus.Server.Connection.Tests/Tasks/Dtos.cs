using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Orcus.Server.Connection.Tasks;
using Orcus.Server.Connection.Tasks.Commands;
using Orcus.Server.Connection.Tasks.Filter;
using Orcus.Server.Connection.Tasks.StopEvents;
using Orcus.Server.Connection.Tasks.Triggers;

namespace Orcus.Server.Connection.Tests.Tasks
{
    public class OperatingSystemFilter : FilterInfo, IXmlSerializable
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

    public class DateTimeTrigger : TriggerInfo, IXmlSerializable
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

    [OrcusCommand("Maze.WakeOnLan", "SystemUtilities;TaskManager")]
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
}