using System;

namespace Console.Shared.Dtos
{
    public class ProcessExitedEventArgs : EventArgs
    {
        public ProcessExitedEventArgs()
        {
        }

        public ProcessExitedEventArgs(int exitCode)
        {
            ExitCode = exitCode;
        }

        public int ExitCode { get; set; }
    }
}