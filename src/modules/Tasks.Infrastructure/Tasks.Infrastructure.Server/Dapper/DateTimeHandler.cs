using System;
using System.Data;
using Dapper;

namespace Tasks.Infrastructure.Server.Dapper
{
    public class DateTimeHandler : SqlMapper.TypeHandler<DateTime>
    {
        public override void SetValue(IDbDataParameter parameter, DateTime value)
        {
            parameter.Value = new DateTimeOffset(value).ToUnixTimeMilliseconds();
        }

        public override DateTime Parse(object value) => DateTimeOffset.FromUnixTimeMilliseconds((long) value).UtcDateTime;
    }
}