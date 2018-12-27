using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Orcus.Server.Connection.Clients
{
    public class ClientGroupDto : IValidatableObject
    {
        public int ClientGroupId { get; set; }
        public string Name { get; set; }
        public List<int> Clients { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Name))
                yield return BusinessErrors.FieldNullOrEmpty(nameof(Name));
        }
    }
}