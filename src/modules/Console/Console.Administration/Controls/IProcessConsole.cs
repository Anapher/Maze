using System;
using System.Threading.Tasks;
using Console.Shared.Dtos;

namespace Console.Administration.Controls
{
    public interface IProcessConsole
    {
        bool IsRunning { get; }
        string Filename { get; }

        event EventHandler<ProcessOutputEventArgs> Output;
        event EventHandler<ProcessOutputEventArgs> Error;
        event EventHandler<ProcessExitedEventArgs> Exited;

        Task WriteInput(string input);
    }
}