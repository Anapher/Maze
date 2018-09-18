using System;
using System.Collections.Generic;
using System.Text;

namespace FileExplorer.Shared.Dtos
{
   public class ExecuteFileDto
    {
        public string FileName { get; set; }
        public string Arguments { get; set; }
        public string WorkingDirectory { get; set; }
        public bool UseShellExecute { get; set; }
        public string Verb { get; set; }
        public bool CreateNoWindow { get; set; }
    }
}
