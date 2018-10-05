using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Orcus.Server.Connection.Tasks.Audience;
using Orcus.Server.Connection.Tasks.Commands;
using Orcus.Server.Connection.Tasks.Conditions;
using Orcus.Server.Connection.Tasks.Execution;
using Orcus.Server.Connection.Tasks.StopEvents;
using Orcus.Server.Connection.Tasks.Transmission;

namespace Orcus.Server.Connection.Tasks
{
    [XmlRoot("task")]
    public class OrcusTask
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("id")]
        public Guid Id { get; set; }

        [XmlElement("isActive")]
        public bool IsActive { get; set; }

        [XmlElement("audience")]
        public AudienceCollection Audience { get; set; }

        [XmlElement("conditions")]
        public IList<ConditionInfo> Conditions { get; set; }

        [XmlElement("transmission")]
        public IList<TransmissionInfo> Transmission { get; set; }

        [XmlElement("execution")]
        public IList<ExecutionInfo> Execution { get; set; }

        [XmlElement("stop")]
        public IList<StopEventInfo> StopEvents { get; set; }

        [XmlElement("commands")]
        public IList<CommandInfo> Commands { get; set; }
    }
}