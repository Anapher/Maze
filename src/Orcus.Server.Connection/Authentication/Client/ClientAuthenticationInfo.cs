using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Orcus.Server.Connection.Authentication.Client
{
    public class ClientAuthenticationInfo : IValidatableObject
    {
        public string Username { get; set; }
        public string OperatingSystem { get; set; }
        public string SystemLanguage { get; set; }
        public bool IsAdministrator { get; set; }
        public string ClientPath { get; set; }
        public byte[] MacAddress { get; set; }
        public string HardwareId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new System.NotImplementedException();
        }
    }
}