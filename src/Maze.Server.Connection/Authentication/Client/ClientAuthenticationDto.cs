using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NuGet.Frameworks;
using NuGet.Versioning;
using Maze.Server.Connection.Extensions;
using Maze.Server.Connection.Utilities;

namespace Maze.Server.Connection.Authentication.Client
{
    public class ClientAuthenticationDto : IValidatableObject
    {
        public string Username { get; set; }
        public string OperatingSystem { get; set; }
        public string SystemLanguage { get; set; }
        public bool IsAdministrator { get; set; }
        public string ClientPath { get; set; }
        public byte[] MacAddress { get; set; }
        public string HardwareId { get; set; }
        public NuGetVersion ClientVersion { get; set; }
        public NuGetFramework Framework { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Username))
                yield return BusinessErrorHelper.FieldNullOrEmpty(nameof(Username));

            if (string.IsNullOrWhiteSpace(OperatingSystem))
                yield return BusinessErrorHelper.FieldNullOrEmpty(nameof(OperatingSystem));

            if (string.IsNullOrWhiteSpace(SystemLanguage))
                yield return BusinessErrorHelper.FieldNullOrEmpty(nameof(SystemLanguage));
            else if (!CultureInfoExtensions.TryGet(SystemLanguage, out _))
                yield return BusinessErrors.InvalidCultureName.ValidateOnMember(nameof(SystemLanguage));

            if (string.IsNullOrWhiteSpace(ClientPath))
                yield return BusinessErrorHelper.FieldNullOrEmpty(nameof(ClientPath));

            if (string.IsNullOrWhiteSpace(HardwareId))
                yield return BusinessErrorHelper.FieldNullOrEmpty(nameof(HardwareId));
            else if (!Hash.TryParse(HardwareId, out _))
                yield return BusinessErrors.InvalidSha256Hash.ValidateOnMember(nameof(HardwareId));

            if (MacAddress == null)
                yield return BusinessErrorHelper.FieldNullOrEmpty(nameof(MacAddress));
            else if (MacAddress.Length != 6)
                yield return BusinessErrors.InvalidMacAddress.ValidateOnMember(nameof(MacAddress));

            if (ClientVersion == null)
                yield return BusinessErrors.FieldNullOrEmpty(nameof(ClientVersion));

            if (Framework == null)
                yield return BusinessErrors.FieldNullOrEmpty(nameof(Framework));
        }
    }
}