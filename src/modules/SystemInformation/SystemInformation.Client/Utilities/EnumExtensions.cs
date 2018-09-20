using System;
using System.ComponentModel;
using System.Linq;

namespace SystemInformation.Client.Utilities
{
    public static class EnumExtensions
    {
        /// <summary>
        ///     Gets an attribute on an enum field value
        /// </summary>
        /// <typeparam name="TEnum">The enum type</typeparam>
        /// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
        /// <param name="value">The enum value</param>
        /// <returns>The attribute of type T that exists on the enum value</returns>
        public static T GetAttribute<TEnum, T>(this TEnum value) where TEnum : Enum where T : Attribute =>
            value.GetType().GetMember(value.ToString()).First().GetCustomAttributes(false).OfType<T>().LastOrDefault();

        public static string GetDescription<TEnum>(this TEnum value) where TEnum : Enum =>
            value.GetAttribute<TEnum, DescriptionAttribute>()?.Description ?? value.ToString().SpaceCamelCase();
    }
}