using System.Diagnostics;
using Tasks.Common.Shared.Commands.Base;

namespace Tasks.Common.Client.Utilities
{
    public static class ProcessCommandInfoExtensions
    {
        public static ProcessStartInfo ToStartInfo(this ProcessCommandInfoBase commandInfo, string filename)
        {
            return new ProcessStartInfo
            {
                Arguments = commandInfo.Arguments,
                CreateNoWindow = commandInfo.CreateNoWindow,
                FileName = filename,
                WorkingDirectory = commandInfo.WorkingDirectory,
                UseShellExecute = commandInfo.UseShellExecute,
                Verb = commandInfo.Verb,

                RedirectStandardError = commandInfo.WaitForExit,
                RedirectStandardOutput = commandInfo.WaitForExit
            };
        }
    }
}
