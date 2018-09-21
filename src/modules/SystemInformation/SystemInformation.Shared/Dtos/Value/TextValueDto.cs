using Newtonsoft.Json;

namespace SystemInformation.Shared.Dtos.Value
{
    public class TextValueDto : ValueDto
    {
        public TextValueDto(string value)
        {
            Value = value;
        }

        public TextValueDto()
        {
        }

        public string Value { get; set; }
        
        public override ValueDtoType Type { get; } = ValueDtoType.Text;
    }
}