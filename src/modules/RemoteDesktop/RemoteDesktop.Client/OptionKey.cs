using System.Collections.Generic;
using System.ComponentModel;

namespace RemoteDesktop.Client
{
    public class OptionKey<T>
    {
        private readonly string _name;
        private readonly TypeConverter _converter;

        public OptionKey(string name)
        {
            _name = name;
            _converter = TypeDescriptor.GetConverter(typeof(T));
        }

        public T GetValue(Dictionary<string, string> options)
        {
            if (options.TryGetValue(_name, out var value))
            {
                return (T) _converter.ConvertFromString(value);
            }

            return default;
        }
    }
}