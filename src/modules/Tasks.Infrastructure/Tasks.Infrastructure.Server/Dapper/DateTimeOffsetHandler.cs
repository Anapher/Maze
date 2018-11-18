using System;
using System.Data;
using Dapper;

namespace Tasks.Infrastructure.Server.Dapper
{
    public class DateTimeOffsetHandler : SqlMapper.TypeHandler<DateTimeOffset>
    {
        public override void SetValue(IDbDataParameter parameter, DateTimeOffset value)
        {
            parameter.Value = value.ToUnixTimeMilliseconds();
        }
        
        public override DateTimeOffset Parse(object value) => DateTimeOffset.FromUnixTimeMilliseconds((long) value);
    }
}