using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Maze.Administration.Library.Deployment;
using Maze.Administration.Library.Models;
using Microsoft.Extensions.Logging;
using Prism.Commands;
using Prism.Mvvm;

namespace Maze.Administration.ViewModels
{
    public class DeployClientBuildViewModel : BindableBase, ILogger
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private DelegateCommand _cancelCommand;
        private DelegateCommand _closeCommand;
        private bool? _dialogResult;
        private bool _isFinished;

        public DeployClientBuildViewModel()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public ILogger LoggerPresenter { get; set; }

        public bool? DialogResult
        {
            get => _dialogResult;
            set => SetProperty(ref _dialogResult, value);
        }

        public bool IsFinished
        {
            get => _isFinished;
            set => SetProperty(ref _isFinished, value);
        }

        public bool Succeeded { get; set; }

        public DelegateCommand CancelCommand
        {
            get { return _cancelCommand ?? (_cancelCommand = new DelegateCommand(() => { _cancellationTokenSource.Cancel(); })); }
        }

        public DelegateCommand CloseCommand
        {
            get { return _closeCommand ?? (_closeCommand = new DelegateCommand(() => { DialogResult = Succeeded; })); }
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            LoggerPresenter?.Log(logLevel, eventId, state, exception, formatter);
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public IDisposable BeginScope<TState>(TState state) => throw new NotSupportedException();

        public async Task<bool> Build(IClientDeployer clientDeployer, IEnumerable<ClientGroupViewModel> groups)
        {
            await Application.Current.Dispatcher.BeginInvoke(new Action(() => { }));
            try
            {
                await clientDeployer.Deploy(groups, this, _cancellationTokenSource.Token);
                this.Log(LogLevel.None, new EventId(1), null);
                Succeeded = true;
            }
            catch (OperationCanceledException)
            {
                this.Log(LogLevel.None, new EventId(-1), null);
                return false;
            }
            catch (Exception e)
            {
                this.Log(LogLevel.None, new EventId(-2), e, null);
                return false;
            }
            finally
            {
                IsFinished = true;
            }
            return true;
        }
    }
}