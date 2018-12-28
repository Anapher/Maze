using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Maze.Administration.Library.Models;
using Maze.Server.Connection.Clients;

namespace Maze.Administration.Core.Clients
{
    public class DefaultClientViewModel : ClientViewModel
    {
        private readonly ConcurrentDictionary<string, object> _attachedProperties;

        private DateTimeOffset _createdOn;
        private string _hardwareId;
        private bool _isSocketConnected;
        private ClientSessionDto _latestSession;
        private string _macAddress;
        private string _operatingSystem;
        private string _systemLanguage;
        private CultureInfo _systemLanguageCulture;
        private string _username;

        public DefaultClientViewModel(ClientDto clientDto)
        {
            ClientId = clientDto.ClientId;
            _attachedProperties = new ConcurrentDictionary<string, object>();

            Update(clientDto);
        }

        public override int ClientId { get; }

        public override string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public override string OperatingSystem
        {
            get => _operatingSystem;
            set => SetProperty(ref _operatingSystem, value);
        }

        public override string MacAddress
        {
            get => _macAddress;
            set => SetProperty(ref _macAddress, value);
        }

        public override string SystemLanguage
        {
            get => _systemLanguage;
            set
            {
                if (SetProperty(ref _systemLanguage, value) && value != null)
                    SetProperty(ref _systemLanguageCulture, CultureInfo.GetCultureInfo(value), nameof(SystemLanguageCulture));
            }
        }

        public override CultureInfo SystemLanguageCulture => _systemLanguageCulture;

        public override string HardwareId
        {
            get => _hardwareId;
            set => SetProperty(ref _hardwareId, value);
        }

        public override DateTimeOffset CreatedOn
        {
            get => _createdOn;
            set => SetProperty(ref _createdOn, value);
        }

        public override bool IsSocketConnected
        {
            get => _isSocketConnected;
            set => SetProperty(ref _isSocketConnected, value);
        }

        public override ClientSessionDto LatestSession
        {
            get => _latestSession;
            set => SetProperty(ref _latestSession, value);
        }

        public override IList<ClientGroupViewModel> Groups { get; } = new ObservableCollection<ClientGroupViewModel>();

        public override object GetValue(string name) => _attachedProperties[name];

        public override bool TryGetValue(string name, out object value) => _attachedProperties.TryGetValue(name, out value);

        public override void SetValue(string name, object value)
        {
            _attachedProperties[name] = value;
        }

        public void Update(ClientDto clientDto)
        {
            Username = clientDto.Username;
            OperatingSystem = clientDto.OperatingSystem;
            MacAddress = clientDto.MacAddress;
            SystemLanguage = clientDto.SystemLanguage;
            HardwareId = clientDto.HardwareId;
            CreatedOn = clientDto.CreatedOn;
            IsSocketConnected = clientDto.IsSocketConnected;
            LatestSession = clientDto.LatestSession;
        }
    }
}