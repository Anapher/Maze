using System;
using System.Collections.Generic;
using System.Globalization;
using Maze.Server.Connection.Clients;
using Prism.Mvvm;

namespace Maze.Administration.Library.Models
{
    public abstract class ClientViewModel : BindableBase
    {
        public abstract int ClientId { get; }
        public abstract string Username { get; set; }
        public abstract string OperatingSystem { get; set; }
        public abstract string MacAddress { get; set; }
        public abstract string SystemLanguage { get; set; }
        public abstract CultureInfo SystemLanguageCulture { get; }
        public abstract string HardwareId { get; set; }
        public abstract DateTimeOffset CreatedOn { get; set; }
        public abstract bool IsSocketConnected { get; set; }
        public abstract ClientSessionDto LatestSession { get; set; }

        public abstract IList<ClientGroupViewModel> Groups { get; }

        public abstract object GetValue(string name);
        public abstract bool TryGetValue(string name, out object value);
        public abstract void SetValue(string name, object value);
    }
}