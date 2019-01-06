using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media.Imaging;
using Maze.Administration.Library.Clients;
using Maze.Administration.Library.ViewModels;
using Prism.Regions;
using RemoteDesktop.Administration.Channels;
using RemoteDesktop.Administration.Channels.Diagnostics;
using RemoteDesktop.Administration.Rest;
using RemoteDesktop.Shared.Options;

namespace RemoteDesktop.Administration.ViewModels
{
    public class RemoteDesktopViewModel : ViewModelBase
    {
        private readonly ITargetedRestClient _restClient;
        private WriteableBitmap _image;
        private string _title;

        public RemoteDesktopViewModel(ITargetedRestClient restClient)
        {
            _restClient = restClient;
        }

        public WriteableBitmap Image
        {
            get => _image;
            set => SetProperty(ref _image, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            var parameters = await RemoteDesktopResource.GetParameters(_restClient);
            var monitor = parameters.Screens.FirstOrDefault(x => x.IsPrimary) ?? parameters.Screens.First();

            var diagnostics = new AtTheMomentDiagonstics();
            diagnostics.UpdateDiagnostics += DiagnosticsOnUpdateDiagnostics;

            var remoteDesktop = await RemoteDesktopResource.CreateScreenChannel(
                new DesktopDuplicationOptions {Monitor = {Value = Array.IndexOf(parameters.Screens, monitor)}}, new x264Options(), _restClient);
            remoteDesktop.PropertyChanged += RemoteDesktopOnPropertyChanged;
            remoteDesktop.Diagonstics = diagnostics;
            await remoteDesktop.StartRecording(_restClient);
        }

        private void DiagnosticsOnUpdateDiagnostics(object sender, DiagnosticData e)
        {
            Title = $"Remote Desktop - {e.Fps} FPS ({e.BytesPerSecond / 1024} KiB/s)";
        }

        private void RemoteDesktopOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var remoteDesktop = (RemoteDesktopChannel) sender;
            switch (e.PropertyName)
            {
                case nameof(RemoteDesktopChannel.Image):
                    Image = remoteDesktop.Image;
                    break;
            }
        }
    }
}