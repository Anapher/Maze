using System;
using System.Collections.Generic;
using System.Text;
using Tasks.Common.Shared.Commands.Base;

namespace Tasks.Common.Shared.Commands
{
    public class DownloadAndExecuteCommandInfo : ProcessCommandInfoBase
    {
        public FileSource FileSource { get; set; }
        public string FileName { get; set; }
    }
}
