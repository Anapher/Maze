using System;
using System.Collections.Concurrent;
using System.Globalization;
using Orcus.Server.Connection.Clients;
using Prism.Mvvm;

namespace Orcus.Administration.Library.Models
{
    public class ClientViewModel : BindableBase
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

        public ClientViewModel(ClientDto clientDto)
        {
            ClientId = clientDto.ClientId;
            _attachedProperties = new ConcurrentDictionary<string, object>();

            Update(clientDto);
        }

        public int ClientId { get; }

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string OperatingSystem
        {
            get => _operatingSystem;
            set => SetProperty(ref _operatingSystem, value);
        }

        public string MacAddress
        {
            get => _macAddress;
            set => SetProperty(ref _macAddress, value);
        }

        public string SystemLanguage
        {
            get => _systemLanguage;
            set
            {
                if (SetProperty(ref _systemLanguage, value) && value != null)
                    SystemLanguageCulture = CultureInfo.GetCultureInfo(value);
            }
        }

        public CultureInfo SystemLanguageCulture
        {
            get => _systemLanguageCulture;
            private set => SetProperty(ref _systemLanguageCulture, value);
        }

        public string HardwareId
        {
            get => _hardwareId;
            set => SetProperty(ref _hardwareId, value);
        }

        public DateTimeOffset CreatedOn
        {
            get => _createdOn;
            set => SetProperty(ref _createdOn, value);
        }

        public bool IsSocketConnected
        {
            get => _isSocketConnected;
            set => SetProperty(ref _isSocketConnected, value);
        }

        public ClientSessionDto LatestSession
        {
            get => _latestSession;
            set => SetProperty(ref _latestSession, value);
        }

        public object GetValue(string name)
        {
            return _attachedProperties[name];
        }

        public bool TryGetValue(string name, out object value)
        {
            return _attachedProperties.TryGetValue(name, out value);
        }

        public void SetValue(string name, object value)
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