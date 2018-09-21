using System;

namespace SystemInformation.Shared.Dtos.Value
{
    public class DateTimeValueDto : ValueDto
    {
        public DateTimeValueDto(DateTimeOffset value)
        {
            Value = value;
        }

        public DateTimeValueDto()
        {
        }

        public DateTimeOffset Value { get; set; }
        public override ValueDtoType Type { get; } = ValueDtoType.DateTime;
    }
}