using System.ComponentModel;

namespace RemoteDesktop.Shared
{
    public class OptionKey<T>
    {
        private readonly TypeConverter _converter;
        private readonly string _name;
        private readonly ComponentOptions _options;

        public OptionKey(string name, ComponentOptions options)
        {
            _name = name;
            _options = options;
            _converter = TypeDescriptor.GetConverter(typeof(T));
        }

        public T Value
        {
            get => GetValue();
            set => SetValue(value);
        }

        public T GetValue()
        {
            if (_options.Options.TryGetValue(_name, out var value)) return (T) _converter.ConvertFromString(value);

            return default;
        }

        public void SetValue(T value)
        {
            _options.Options[_name] = _converter.ConvertToString(value);
        }
    }
}