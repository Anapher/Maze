using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Orcus.Administration.ControllerExtensions;
using Orcus.Administration.Library.Clients;
using Orcus.Utilities;
using Prism.Mvvm;
using TaskManager.Administration.Rest;
using TaskManager.Administration.Utilities;
using TaskManager.Shared.Channels;
using TaskManager.Shared.Dtos;
using Unclassified.TxLib;

namespace TaskManager.Administration.ViewModels
{
    public class PropertiesViewModel : BindableBase, IDisposable
    {
        private readonly IPackageRestClient _restClient;
        private List<ActiveConnectionDto> _activeConnections;
        private int _exitCode;
        private ChangingProcessPropertiesDto _propertiesDto;
        private ProcessStatus _status;
        private TimeSpan _totalProcessorTime;
        private readonly DispatcherTimer _refreshTimer;
        private CallTransmissionChannel<IProcessWatcher> _processWatcher;
        private bool _isDisposed;
        private bool _refreshedActiveConnections;

        public PropertiesViewModel(ProcessPropertiesDto properties, ProcessViewModel process, IPackageRestClient restClient)
        {
            _restClient = restClient;
            StaticPropertiesDto = properties;
            PropertiesDto = properties;
            Process = process;

            if (process.ParentProcessId != 0)
            {
                string parentProcess;
                if (process.ParentViewModel != null)
                    parentProcess = process.Name;
                else parentProcess = "<" + Tx.T("TaskManager:NotAvailable") + ">";

                ParentProcessString = $"{parentProcess} ({process.ParentProcessId})";
            }

            Icon = ImageUtilities.GetBitmapImage(properties.Icon);
            Status = properties.Status;

            _refreshTimer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(5)};
            _refreshTimer.Tick += RefreshTimerOnTick;

            InitializeChannel().Forget();
            UpdateActiveConnections().Forget();
        }

        public ProcessViewModel Process { get; }
        public ProcessPropertiesDto StaticPropertiesDto { get; }

        public ChangingProcessPropertiesDto PropertiesDto
        {
            get => _propertiesDto;
            private set
            {
                if (SetProperty(ref _propertiesDto, value)) TotalProcessorTime = value.TotalProcessorTime;
            }
        }

        public List<ActiveConnectionDto> ActiveConnections
        {
            get => _activeConnections;
            set => SetProperty(ref _activeConnections, value);
        }

        public ProcessStatus Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public int ExitCode
        {
            get => _exitCode;
            set => SetProperty(ref _exitCode, value);
        }

        public TimeSpan TotalProcessorTime
        {
            get => _totalProcessorTime;
            set => SetProperty(ref _totalProcessorTime, value);
        }

        public string ParentProcessString { get; }
        public BitmapImage Icon { get; }

        private async Task UpdateActiveConnections()
        {
            ActiveConnections = await ProcessesResource.GetConnections(Process.Id, _restClient);
        }

        private async Task InitializeChannel()
        {
            _processWatcher = await TaskManagerResource.GetProcessWatcher(_restClient);
            if (_isDisposed)
            {
                _processWatcher.Dispose();
                return;
            }

            await _processWatcher.Interface.WatchProcess(Process.Id);
            _processWatcher.Interface.Exited += InterfaceOnExited;

            _refreshTimer.Start();
        }

        private void InterfaceOnExited(object sender, ProcessExitedEventArgs e)
        {
            _refreshTimer.Stop();
            Status = ProcessStatus.Exited;
            ExitCode = e.ExitCode;
        }

        private async void RefreshTimerOnTick(object sender, EventArgs e)
        {
            if (_refreshedActiveConnections)
            {
                UpdateActiveConnections().Forget();
                _refreshedActiveConnections = true;
            }
            else _refreshedActiveConnections = false;

            PropertiesDto = await _processWatcher.Interface.GetInfo();
        }

        public void Dispose()
        {
            _isDisposed = true;

            _refreshTimer.Stop();
            _processWatcher?.Dispose();
        }
    }
}