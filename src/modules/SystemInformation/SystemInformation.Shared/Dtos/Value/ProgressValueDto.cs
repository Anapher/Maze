namespace SystemInformation.Shared.Dtos.Value
{
    public class ProgressValueDto : ValueDto
    {
        public ProgressValueDto(double value)
        {
            Value = value;
        }

        public ProgressValueDto(double value, double maximum)
        {
            Value = value;
            Maximum = maximum;
        }

        public ProgressValueDto()
        {
        }

        public double Value { get; set; }
        public double Maximum { get; set; } = 1;

        public override ValueDtoType Type { get; } = ValueDtoType.Progress;
    }
}