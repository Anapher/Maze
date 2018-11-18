using System;
using System.Data;
using Dapper;

namespace Tasks.Infrastructure.Server.Hooks
{
    public class DateTimeOffsetHandler : SqlMapper.TypeHandler<DateTimeOffset>
    {
        public override void SetValue(IDbDataParameter parameter, DateTimeOffset value)
        {
            parameter.Value = value.ToUnixTimeMilliseconds();
        }
        
        public override DateTimeOffset Parse(object value) => DateTimeOffset.FromUnixTimeMilliseconds((long) value);
    }

    public class DateTimeHandler : SqlMapper.TypeHandler<DateTime>
    {
        public override void SetValue(IDbDataParameter parameter, DateTime value)
        {
            parameter.Value = new DateTimeOffset(value).ToUnixTimeMilliseconds();
        }

        public override DateTime Parse(object value) => DateTimeOffset.FromUnixTimeMilliseconds((long) value).UtcDateTime;
    }
}