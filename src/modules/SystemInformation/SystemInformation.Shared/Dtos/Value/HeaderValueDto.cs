namespace SystemInformation.Shared.Dtos.Value
{
    public class HeaderValueDto : ValueDto
    {
        public static HeaderValueDto Instance => new HeaderValueDto();
        public override ValueDtoType Type { get; } = ValueDtoType.Header;
    }
}