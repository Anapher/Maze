namespace SystemInformation.Shared.Dtos.Value
{
    public class BoolValueDto : ValueDto
    {
        public BoolValueDto(bool value)
        {
            Value = value;
        }

        public BoolValueDto()
        {
        }

        public bool Value { get; set; }
        public override ValueDtoType Type { get; } = ValueDtoType.Boolean;
    }
}