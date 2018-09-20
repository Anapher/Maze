namespace SystemInformation.Shared.Dtos.Value
{
    public class DataSizeValueDto : ValueDto
    {
        public DataSizeValueDto(long value)
        {
            Value = value;
        }

        public DataSizeValueDto()
        {
        }

        public long Value { get; set; }
        public override ValueDtoType Type { get; } = ValueDtoType.DataSize;
    }
}