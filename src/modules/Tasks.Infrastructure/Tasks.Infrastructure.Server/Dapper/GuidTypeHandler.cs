using System;
using System.Data;
using Dapper;

namespace Tasks.Infrastructure.Server.Dapper
{
    public class GuidTypeHandler : SqlMapper.TypeHandler<Guid>
    {
        public override Guid Parse(object value)
        {
            var inVal = (byte[]) value;
            return Parse(inVal);
        }

        public override void SetValue(IDbDataParameter parameter, Guid value)
        {
            var inVal = value.ToByteArray();
            byte[] outVal =
            {
                inVal[3], inVal[2], inVal[1], inVal[0], inVal[5], inVal[4], inVal[7], inVal[6], inVal[8], inVal[9], inVal[10], inVal[11],
                inVal[12], inVal[13], inVal[14], inVal[15]
            };
            parameter.Value = outVal;
        }

        public static Guid Parse(byte[] data)
        {
            byte[] outVal =
            {
                data[3], data[2], data[1], data[0], data[5], data[4], data[7], data[6], data[8], data[9], data[10], data[11],
                data[12], data[13], data[14], data[15]
            };
            return new Guid(outVal);
        }
    }
}