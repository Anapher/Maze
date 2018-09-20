using System.Globalization;

namespace SystemInformation.Shared.Dtos.Value
{
    public class CultureValueDto : ValueDto
    {
        public CultureValueDto(CultureInfo value)
        {
            Value = value.TwoLetterISOLanguageName;
        }

        public CultureValueDto()
        {
        }

        public string Value { get; set; }

        public override ValueDtoType Type { get; } = ValueDtoType.Culture;
    }
}