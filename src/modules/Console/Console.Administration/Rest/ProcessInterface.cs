using System;
using System.Threading.Tasks;
using Console.Administration.Controls;
using Console.Shared.Channels;
using Console.Shared.Dtos;
using Orcus.Administration.ControllerExtensions;

namespace Console.Administration.Rest
{
    public class ProcessInterface : IProcessConsole
    {
        public ProcessInterface(CallTransmissionChannel<IProcessChannel> processChannel)
        {
            processChannel.SuspendSubscribing();
            processChannel.Interface.Error += InterfaceOnError;
            processChannel.Interface.Output += InterfaceOnOutput;
            processChannel.Interface.Exited += InterfaceOnExited;
            processChannel.ResumeSubscribing();

            ProcessChannel = processChannel;
        }

        public CallTransmissionChannel<IProcessChannel> ProcessChannel { get; }

        public bool IsRunning { get; private set; }
        public string Filename { get; private set; }

        public event EventHandler<ProcessOutputEventArgs> Output;
        public event EventHandler<ProcessOutputEventArgs> Error;
        public event EventHandler<ProcessExitedEventArgs> Exited;

        public Task WriteInput(string input) => ProcessChannel.Interface.WriteInput(input);

        public Task StartProcess(string filename, string arguments)
        {
            return ProcessChannel.Interface.StartProcess(filename, arguments).ContinueWith(task =>
            {
                if (!task.IsCanceled && !task.IsFaulted)
                {
                    Filename = filename;
                    IsRunning = true;
                }
            });
        }

        private void InterfaceOnExited(object sender, ProcessExitedEventArgs e)
        {
            Exited?.Invoke(this, e);
        }

        private void InterfaceOnOutput(object sender, ProcessOutputEventArgs e)
        {
            Output?.Invoke(this, e);
        }

        private void InterfaceOnError(object sender, ProcessOutputEventArgs e)
        {
            Error?.Invoke(this, e);
        }

        public void Dispose()
        {
            ProcessChannel?.Dispose();
            IsRunning = false;
            Exited?.Invoke(this, new ProcessExitedEventArgs(-1));
        }
    }
}