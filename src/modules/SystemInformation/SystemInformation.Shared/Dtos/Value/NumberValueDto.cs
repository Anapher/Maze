namespace SystemInformation.Shared.Dtos.Value
{
    public class NumberValueDto : ValueDto
    {
        public NumberValueDto(long value)
        {
            Value = value;
        }

        public NumberValueDto()
        {
        }

        public long Value { get; set; }
        public override ValueDtoType Type { get; } = ValueDtoType.Number;
    }
}