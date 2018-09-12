using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TaskManager.Shared.Dtos
{
    public class ProcessDto
    {
        public ProcessDto()
        {
            BinaryProperties = new Dictionary<string, byte[]>();
            StringProperties = new Dictionary<string, string>();
            IntegerProperties = new Dictionary<string, long>();
            DateTimeProperties = new Dictionary<string, DateTimeOffset>();
            BooleanProperties = new Dictionary<string, bool>();
        }

        public int ProcessId { get; set; }

        public Dictionary<string, byte[]> BinaryProperties { get; set; }
        public Dictionary<string, string> StringProperties { get; set; }
        public Dictionary<string, long> IntegerProperties { get; set; }
        public Dictionary<string, DateTimeOffset> DateTimeProperties { get; set; }
        public Dictionary<string, bool> BooleanProperties { get; set; }

        public void Add(string key, object value)
        {
            if (value == null) return;

            if (value is string stringValue)
                StringProperties.Add(key, stringValue);
            else if (value is byte[] binaryValue)
                BinaryProperties.Add(key, binaryValue);
            else if (value is bool boolValue)
                BooleanProperties.Add(key, boolValue);
            else if (value is DateTimeOffset dateTimeOffsetValue)
                DateTimeProperties.Add(key, dateTimeOffsetValue);
            else if (value is DateTime dateTimeValue)
                DateTimeProperties.Add(key, dateTimeValue);
            else
            {
                IntegerProperties.Add(key, Convert.ToInt64(value));
            }
        }

        public bool TryGetProperty(string key, out byte[] value)
        {
            return BinaryProperties.TryGetValue(key, out value);
        }

        public bool TryGetProperty(string key, out string value)
        {
            return StringProperties.TryGetValue(key, out value);
        }

        public bool TryGetProperty(string key, out bool value)
        {
            return BooleanProperties.TryGetValue(key, out value);
        }

        public bool TryGetProperty(string key, out DateTimeOffset value)
        {
            return DateTimeProperties.TryGetValue(key, out value);
        }

        public bool TryGetProperty(string key, out long value)
        {
            return IntegerProperties.TryGetValue(key, out value);
        }

        public bool TryGetProperty(string key, out DateTime value)
        {
            var result = DateTimeProperties.TryGetValue(key, out var offsetValue);
            value = offsetValue.LocalDateTime;
            return result;
        }

        public bool TryGetProperty<T>(string key, out T value)
        {
            if (IntegerProperties.TryGetValue(key, out var val))
            {
                value = (T) Convert.ChangeType(val, typeof(T));
                return true;
            }

            if (StringProperties.TryGetValue(key, out var stringVal))
            {
                value = (T) TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(stringVal);
                return true;
            }

            value = default;
            return false;
        }
    }

    public enum ProcessType
    {
        None,
        UserProcess,
        NetAssembly,
        Service,
        Immersive
    }
}