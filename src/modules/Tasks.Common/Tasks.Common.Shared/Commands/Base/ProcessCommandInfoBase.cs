using System;
using System.Collections.Generic;
using System.Text;
using Tasks.Infrastructure.Core.Commands;

namespace Tasks.Common.Shared.Commands.Base
{
    public abstract class ProcessCommandInfoBase : CommandInfo
    {
        public string WorkingDirectory { get; set; }
        public string Arguments { get; set; }
        public string Verb { get; set; }
        public bool UseShellExecute { get; set; }
        public bool CreateNoWindow { get; set; }

        public bool WaitForExit { get; set; }
    }
}
