using System;
using System.Collections.Generic;
using System.Management;
using SystemInformation.Shared.Dtos;
using SystemInformation.Shared.Dtos.Value;

namespace SystemInformation.Client.Utilities
{
    public static class SystemInfoValueExtensions
    {
        public static void TryAdd<T>(this IList<SystemInfoDto> dtos, string category, ManagementObject managementObject, string name,
            Func<T, ValueDto> getValue)
        {
            if (managementObject.TryGetProperty(name, out T value))
            {
                var dto = new SystemInfoDto {Name = $"@{category}.{name}", Category = category, Value = getValue(value)};
                dtos.Add(dto);
            }
        }

        public static void TryAddDateTime(this IList<SystemInfoDto> dtos, string category, ManagementObject managementObject, string name)
        {
            if (managementObject.TryGetDateTime(name, out DateTimeOffset value))
            {
                var dto = new SystemInfoDto {Name = $"@{category}.{name}", Category = category, Value = new DateTimeValueDt(value)};
                dtos.Add(dto);
            }
        }

        public static void TryAdd<T>(this IList<SystemInfoDto> dtos, string category, ManagementObject managementObject, string name)
        {
            if (managementObject.TryGetProperty(name, out T value))
            {
                var dto = new SystemInfoDto {Name = $"@{category}.{name}", Category = category};

                switch (value)
                {
                    case var val when val is string stringValue:
                        dto.Value = new TextValueDto(stringValue);
                        break;
                    case var val when val is ulong ulongValue:
                        dto.Value = new NumberValueDto((long) ulongValue);
                        break;
                    case var val when val is uint uintValue:
                        dto.Value = new NumberValueDto(uintValue);
                        break;
                    case var val when val is bool boolValue:
                        dto.Value = new BoolValueDto(boolValue);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                dtos.Add(dto);
            }
        }
    }
}