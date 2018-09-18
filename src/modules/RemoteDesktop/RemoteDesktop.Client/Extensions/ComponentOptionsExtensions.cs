using System;
using RemoteDesktop.Shared;

namespace RemoteDesktop.Client.Extensions
{
    public static class ComponentOptionsExtensions
    {
        public static T To<T>(this ComponentOptions options) where T : ComponentOptions, new()
        {
            var result = new T();
            if (result.ComponentName != options.ComponentName)
                throw new ArgumentException($"Cannot convert options of {options.ComponentName} to options of {result.ComponentName}.");

            foreach (var val in options.Options)
            {
                result.Options.Add(val.Key, val.Value);
            }

            return result;
        }
    }
}