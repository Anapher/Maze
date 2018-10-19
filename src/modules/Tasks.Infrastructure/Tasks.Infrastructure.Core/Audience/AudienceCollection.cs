using System.Collections.Generic;
using Orcus.Server.Connection.Commanding;

namespace Tasks.Infrastructure.Core.Audience
{
    public class AudienceCollection : List<CommandTarget>
    {
        public bool IsAll { get; set; }
        public bool IncludesServer { get; set; }
    }
}