using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Linq;

namespace Maze.Server.Connection.Clients
{
    public class ClientConfigurationDto : IValidatableObject
    {
        public int ClientConfigurationId { get; set; }
        public int? ClientGroupId { get; set; }

        public string Content { get; set; }

        public DateTimeOffset UpdatedOn { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Content))
                yield return BusinessErrors.FieldNullOrEmpty(nameof(Content));

            var jsonString = Content.Trim();
            if (!jsonString.StartsWith("{") || !jsonString.EndsWith("}"))
                yield return BusinessErrors.ClientConfigurations.InvalidJson;

            var parseSucceeded = false;
            try
            {
                JToken.Parse(Content);
                parseSucceeded = true;
            }
            catch (Exception)
            {
                // ignored
            }

            if (!parseSucceeded)
                yield return BusinessErrors.ClientConfigurations.InvalidJson;
        }
    }
}