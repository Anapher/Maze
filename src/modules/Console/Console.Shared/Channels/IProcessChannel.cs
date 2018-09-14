using System;
using System.Threading.Tasks;
using Console.Shared.Dtos;

namespace Console.Shared.Channels
{
    public interface IProcessChannel
    {
        event EventHandler<ProcessOutputEventArgs> Output;
        event EventHandler<ProcessOutputEventArgs> Error;
        event EventHandler<ProcessExitedEventArgs> Exited;

        Task StartProcess(string filename, string arguments);
        Task WriteInput(string input);
    }
}