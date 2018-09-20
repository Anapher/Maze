using System;

namespace SystemInformation.Shared.Dtos.Value
{
    public class DateTimeValueDt : ValueDto
    {
        public DateTimeValueDt(DateTimeOffset value)
        {
            Value = value;
        }

        public DateTimeValueDt()
        {
        }

        public DateTimeOffset Value { get; set; }
        public override ValueDtoType Type { get; } = ValueDtoType.DateTime;
    }
}