using System.Collections.Generic;
using Orcus.Server.Connection.Commanding;

namespace Orcus.Server.Connection.Tasks.Audience
{
    public class AudienceCollection : List<CommandTarget>
    {
        public bool IsAll { get; set; }
        public bool IncludesServer { get; set; }
    }
}